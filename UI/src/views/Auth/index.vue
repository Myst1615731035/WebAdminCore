<template>
	<div class="container-grid">
		<el-card class="data-list role-list">
			<template #header>
				<div class="card-header">
					<span>角色列表</span>
					<vxe-button @click="InitData()">刷新</vxe-button>
				</div>
			</template>
			<vxe-list ref="roleList" :data="roles">
				<template #default="{ items }">
					<p class="list-item" v-for="(item, index) in items" :key="index" @click="SelectRole(item)" :class="item.Id == curRole.Id ? 'active' : ''">
						<i :class="item.Id == curRole.Id ? 'fa fa-angle-right' : ''"></i>
						<span>{{ item.Name }}</span>
					</p>
				</template>
			</vxe-list>
		</el-card>
		<el-card class="data-list permission-list">
			<template #header>
				<div class="card-header">
					<span>权限列表</span>
					<vxe-button @click="SaveRoleAuth()">保存</vxe-button>
				</div>
			</template>
			<el-tree
				ref="menutree"
				:props="treeProps"
				:data="menus"
				node-key="Id"
				:show-checkbox="true"
				:default-checked-keys="signIds"
				default-expand-all
				:highlight-current="true"
				:indent="30"
			>
				<template #default="{ node, data }">
					<div class="custom-tree-node">
						<div class="menu-title">
							{{ data.Name }}
							<vxe-button v-if="!!data.Buttons && data.Buttons.length > 0" @click="SelectBtns(data)" content="全选" style="margin-left: 0.625rem;"></vxe-button>
							<vxe-button v-if="!!data.Buttons && data.Buttons.length > 0" @click="SelectBtns(data, true)" content="反选"></vxe-button>
						</div>
						<vxe-checkbox-group v-if="!!data.Buttons && data.Buttons.length > 0" v-model="signIds" class="btn-list">
							<vxe-checkbox v-for="t in data.Buttons" :label="t.Id" :content="t.Name" :key="t.Id"></vxe-checkbox>
						</vxe-checkbox-group>
					</div>
				</template>
			</el-tree>
		</el-card>
	</div>
</template>

<script>
export default {
	created() {
		this.InitData();
	},
	data() {
		return {
			menus: [],
			signIds: [],
			roles: [],
			treeProps: { label: 'Name', children: 'Children', class: 'tree-item' },
			curRole: {}
		};
	},
	methods: {
		InitData() {
			this.GetRoles();
			this.GetMenus();
		},
		// 获取角色列表
		GetRoles() {
			this.$post(this.serverApi.sysRole.list, { isAll: true }).then(res => {
				if (res.success) {
					this.roles = res.data.response;
					if (this.roles.length > 0) {
						if (!!!this.roles.find(t => t.Id == this.curRole.Id)) this.curRole = this.roles[0];
						this.GetRolePermission(this.curRole.Id);
					}
				}
			});
		},
		GetMenus() {
			this.$postPage(this.serverApi.permission.list, { isAll: true }).then(res => (this.menus = res));
		},
		// 获取角色的授权的权限ID
		GetRolePermission(id) {
			this.$post(`${this.serverApi.authority.rolePermission}?roleId=${id}`).then(res => (this.signIds = res.success ? res.data : []));
		},
		// 选择角色触发事件
		SelectRole(role) {
			if (role.Id != this.curRole.Id) {
				this.curRole = role;
				this.GetRolePermission(this.curRole.Id);
			}
		},
		// 保存角色授权
		SaveRoleAuth() {
			if (!!this.curRole.Id) {
				// 获取所有已选择或半选的数据
				var result = Array.from(new Set(this.signIds.concat(this.$refs.menutree.getCheckedKeys().concat(this.$refs.menutree.getHalfCheckedKeys()))));
				this.$post(`${this.serverApi.authority.saveRoleAuth}?roleId=${this.curRole.Id}`, result).then(res => {
					if (res.success) this.GetRolePermission(this.curRole.Id);
					this.$alertRes(res);
				});
			}
		},
		SelectBtns(menu, reverse = false) {
			var btnIds = menu.Buttons.map(t => t.Id);
			if (reverse) {
				var res = btnIds.filter(t => !this.signIds.includes(t));
				this.signIds = this.signIds.filter(t => !btnIds.includes(t)).concat(res);
			} else this.signIds = Array.from(new Set(this.signIds.concat(btnIds)));
		}
	}
};
</script>

<style>
.card-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
}
.data-list {
	display: inline-block;
	height: 100%;
	padding: 0.625rem;
}
.role-list {
	width: 35%;
}
.permission-list {
	width: 65%;
}
.permission-list .el-tree-node__content {
	height: 1.875rem;
	line-height: 1.875rem;
}
.list-item {
	display: block;
	padding: 0.5rem;
	height: 2.25rem;
	line-height: 1.125rem;
	width: 100%;
	text-align: left;
}
.list-item:hover {
	background-color: #ebf5ff;
}
.active {
	background-color: #ebf5ff;
	color: black;
	font-weight: 600;
}
.custom-tree-node {
	display: block;
	width: 100%;
}
.custom-tree-node .menu-title {
	float: left;
	display: inline;
}
.custom-tree-node .btn-list {
	float: right;
	padding-right: 2rem;
}
.btn-list .vxe-checkbox--icon {
	display: none !important;
}

.btn-list .vxe-checkbox--label {
	padding: 0.25rem 0.6rem;
	display: inline-block;
	border-style: solid;
	border-color: #dcdfe6;
	border-width: 1px;
	border-radius: 0.375rem;
	max-width: 50em;
}
.custom-tree-node .vxe-button {
	height: 100% !important;
	padding: 0.25rem 0.6rem !important;
}
.custom-tree-node .vxe-checkbox > input:checked + .vxe-checkbox--icon + .vxe-checkbox--label {
	background-color: #409eff;
	color: white;
}
</style>

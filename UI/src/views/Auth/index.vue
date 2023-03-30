<template>
	<div class="container-grid">
		<el-card class="data-list role-list">
			<template #header>
				<div class="card-header">
					<span>角色列表</span>
					<vxe-button @click="GetRoles()">刷新</vxe-button>
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
				ref="permissionList"
				:props="treeProps"
				:data="permissions"
				:default-checked-keys="checkedKeys"
				show-checkbox
				node-key="Id"
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
						<vxe-checkbox-group v-model="checkedKeys" v-if="!!data.Buttons && data.Buttons.length > 0" class="btn-list">
							<vxe-checkbox v-for="t in data.Buttons" :label="t.Id" :content="t.Name" :key="t.Id"></vxe-checkbox>
						</vxe-checkbox-group>
					</div>
				</template>
			</el-tree>
		</el-card>
	</div>
</template>

<script>
let self;
export default {
	created() {
		self = this;
		self.GetRoles();
	},
	data() {
		return { roles: [], permissions: [], checkedKeys: [], treeProps: { label: 'Name', children: 'Children', class: 'tree-item' }, curRole: {} };
	},
	methods: {
		// 获取角色列表
		GetRoles() {
			self.roles = [];
			self.permission = [];
			self.$post(self.$store.state.serverApi.sysRole.list, { isAll: true }).then(res => {
				if (res.success) {
					self.roles = res.data.response;
					if (self.roles.length > 0) {
						if (IsEmpty(self.roles.find(t => t.Id == self.curRole.Id))) self.curRole = self.roles[0];
						self.GetRolePermission(self.curRole.Id);
					}
				}
			});
		},
		// 获取单个角色的授权列表
		GetRolePermission(id) {
			self.permissions = [];
			self.checkedKeys = [];
			self.$post(self.$store.state.serverApi.authority.rolePermission, { id }).then(res => {
				if (res.success) {
					self.permissions = Object.freeze(res.data.permissionTree);
					self.checkedKeys = res.data.hasAuthed || [];
				}
			});
		},
		// 选择角色触发事件
		SelectRole(role) {
			if (role.Id != self.curRole.Id) {
				self.curRole = role;
				self.GetRolePermission(self.curRole.Id);
			}
		},
		// 保存角色授权
		SaveRoleAuth() {
			if (!!self.curRole.Id) {
				var permissionSelected = self.$refs.permissionList.getCheckedKeys().concat(self.$refs.permissionList.getHalfCheckedKeys());
				permissionSelected = Array.from(new Set(permissionSelected.concat(this.checkedKeys)));
				self.$post(self.serverApi.authority.saveRoleAuth, { roleId: self.curRole.Id, list: permissionSelected }).then(res => {
					self.GetRolePermission(self.curRole.Id);
					self.$alertRes(res);
				});
			}
		},
		SelectBtns(menu, reverse = false) {
			var btnIds = menu.Buttons.map(t => t.Id);
			if (reverse) {
				btnIds.forEach(t => {
					var checkIndex = this.checkedKeys.indexOf(t);
					if (checkIndex > -1) this.checkedKeys.splice(checkIndex, 1);
					else this.checkedKeys.push(t);
				});
			} else {
				var list = this.checkedKeys.concat(btnIds);
				this.checkedKeys = Array.from(new Set(this.checkedKeys.concat(btnIds)));
			}
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

.custom-tree-node .vxe-checkbox--icon {
	display: none !important;
}

.custom-tree-node .vxe-checkbox--label {
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
</style>

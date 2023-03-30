<template>
	<vxe-form class="user-info" v-bind="formOption">
		<template #avatar="{data}"></template>
		<template #updatebtn>
			<vxe-button status="primary" content="更新" @click="UpdateUserInfo"></vxe-button>
		</template>
	</vxe-form>
</template>

<script>
export default {
	created() {
		this.GetUserInfo();
	},
	data() {
		return {
			formOption: {
				data: { Id: '', Name: '', Password: '', NewPassword: '', ConfirmPassword: '', Avatar: '', Remark: '' },
				className: 'form-content',
				titleOverflow: true,
				titleWidth: 120,
				titleAlign: 'right',
				preventSubmit: true,
				items: [
					{ field: 'Id', title: '主键', visible: false },
					{ field: 'Name', title: '名称', span: 24, itemRender: { name: '$input' } },
					{ field: 'Password', title: '旧密码', span: 24, itemRender: { name: '$input', props: { type: 'password' } } },
					{ field: 'NewPassword', title: '新密码', span: 24, itemRender: { name: '$input', props: { type: 'password' } } },
					{ field: 'ConfirmPassword', title: '确认密码', span: 24, itemRender: { name: '$input', props: { type: 'password' } } },
					{ field: 'Avatar', title: '头像', span: 24, slots: { default: 'avatar' } },
					{ field: 'Remark', title: '备注', span: 24, itemRender: { name: '$textarea', props: { rows: 4 } } },
					{ span: 24, align: 'right', className: 'update-btn', slots: { default: 'updatebtn' } }
				]
			}
		};
	},
	methods: {
		GetUserInfo() {
			this.$post(this.serverApi.user.getInfo, { token: this.$store.state.loginInfo.token }).then(res => {
				if (res.success) {
					this.formOption.data = res.data;
					this.formOption.data.Id = res.data.UserId;
				}
			});
		},
		async UpdateUserInfo() {
			var data = await this.ValidData();
			if(!!data){
				this.$post(this.serverApi.sysUser.save, data).then(res => {
					this.$alertRes(res);
					if (res.success) this.GetUserInfo();
				});
			}
		},
		async ValidData() {
			var msg = '';
			var data = this.formOption.data;
			// 如果更新密码，先校验密码
			if (!IsEmpty(data.NewPassword)) {
				if (IsEmpty(data.Password)) msg = '更新密码，需要对原始密码进行验证，请输入原始密码';
				if (data.NewPassword != data.ConfirmPassword) msg = '两次输入的新密码不一致，请确认';
				if(!!msg) {
					this.$message({ content: msg, status: 'warning' });
					return null;
				}	
			} else {
				data.Password = '';
				data.NewPassword = '';
			}
			return data;
		}
	}
};
</script>

<style>
.user-info {
	padding: 2.5rem;
	width: 70%;
}
.update-btn {
	padding-right: 5rem !important;
}
</style>

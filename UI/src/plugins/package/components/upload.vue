<template>
	<vxe-modal ref="modal" v-bind="modalConfig" @show="open" @close="close" :before-hide-method="beforeHideMethod">
		<template #footer>
			<vxe-button content="清空" status="danger" @click="clear({ type: 'cancel' })"></vxe-button>
			<vxe-button content="上传" status="primary" @click="confirm({ type: 'confirm' })"></vxe-button>
		</template>
		<el-upload ref="upload" v-model:file-list="fileList" v-bind="uploadConfig" drag @preview="preview" @change="change" @exceed="exceed" @remove="remove">
			<div v-if="type == 'file'">
				<el-icon class="el-icon--upload"><upload-filled /></el-icon>
				<div class="el-upload__text">Drop file here or click to upload</div>
				<div v-if="!!props.tip" class="el-upload__tip">{{ props.tip || '' }}</div>
			</div>
			<el-icon v-else><Plus /></el-icon>
		</el-upload>
	</vxe-modal>
</template>

<script>
export default {
	props: {
		show: { type: Boolean, default: false },
		// 上传组件的类型，image / file
		type: { type: String, default: 'file' },
		props: { type: Object, default: {} },
	},
	emits: ['open', 'confirm', 'close', 'submit', 'preview', 'change', 'exceed', 'remove'],
	data() {
		return { modalShow: this.show, fileList: [] };
	},
	watch: {
		modalShow() {
			this.$emit('update:show', this.modalShow);
		},
	},
	computed: {
		modalConfig() {
			return {
				title: this.props.title || '上传',
				type: 'confirm',
				showFooter: true,
				resize: true,
				width: this.props.width || 400,
				height: this.props.height || 600,
				cancelButtonText: '清空',
				confirmButtonText: '上传',
				marginSize: -1,
				mask: false,
			};
		},
		uploadConfig() {
			const defaultConfig = { multiple: false };
			var config = Object.assign(defaultConfig, this.props);
			if (this.type == 'image') config.listType = 'picture-card';
			else config.listType = 'text';
			if (!config.multiple) if (!!!config.limit && config.limit != 0) config.limit = 1;
			config.autoUpload = false;
			return config;
		},
	},
	methods: {
		open({ type }) {
			if (IsEmpty(this.uploadConfig.action)) console.error('文件上传路径为空，组件无法使用');
			this.$emit('open', { type });
		},
		async confirm({ type, $event }) {
			if (!!this.fileList && this.fileList.length > 0) {
				if (IsNotEmpty(this.uploadConfig.action)) {
					var formData = new FormData();
					this.fileList.forEach((t) => formData.append(t.name, t.raw));
					this.$putFile(this.uploadConfig.action, formData).then((res) => {
						// this.$showRes(res);
						if (res.success) this.$refs.modal.close();
						else this.$refs.modal.beforeHideMethod(true);
						this.$emit('submit', res);
					});
				}
				return;
			} else this.$message({ content: '未获取到需要上传的文件', status: 'warning' });
		},
		async clear({ type }) {
			this.$refs.upload.clearFiles();
		},
		close({ type, $event }) {
			this.$emit('close', { type, $event });
		},
		preview(uf, ufs) {
			this.$emit('preview', { uf, ufs });
		},
		change(uf, ufs) {
			this.fileList = ufs;
			this.$emit('change', { uf, ufs });
		},
		exceed(fs, uf, ufs) {
			this.$message({ content: `最大上传数量：${this.uploadConfig.limit}`, status: 'warning' });
			this.$emit('exceed', { fs, uf, ufs });
		},
		remove(uf, ufs) {
			this.fileList = ufs;
			this.$emit('remove', { uf, ufs });
		},
	},
};
</script>

<style>
.el-upload-dragger {
	height: 100%;
	padding-top: 58px;
	padding-bottom: 58px;
}
</style>

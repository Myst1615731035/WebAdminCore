<template>
	<div class="vxe-tiny-editor">
		<p class="editor-title">
			{{ field }}
			<el-tooltip placement="top" :content="splitBtnTooltip">
				<vxe-checkbox v-model="module.splitBtnChecked" class="code-btn" :class="module.splitBtnChecked ? 'code-btn-bg' : ''" @change="SplitBtnClick">
					<i class="fa fa-retweet" />
				</vxe-checkbox>
			</el-tooltip>
			<el-tooltip placement="top" :content="switchBtnTooltip">
				<vxe-checkbox v-model="module.switchBtnChecked" class="code-btn" :class="module.switchBtnChecked ? 'code-btn-bg' : ''" @change="SwitchBtnClick">
					<i class="fa fa-code" />
				</vxe-checkbox>
			</el-tooltip>
		</p>
		<div class="half" :style="module.editor">
			<editor v-model="data[field]" :init="config" :tinymceScriptSrc="jsSrc"></editor>
		</div>
		<div class="half" :style="module.mirror"><mirror ref="mirror" :data="data" :field="field" :props="{ language: 'html', height: height }"></mirror></div>
	</div>
</template>

<script>
const menubar = 'insert edit view format table',
	plugins =
		'advlist quickbars anchor autolink autosave code codesample directionality emoticons fullscreen image insertdatetime link lists media nonbreaking pagebreak preview save searchreplace table template visualblocks visualchars wordcount help',
	toolbar = [
		'removeformat bold italic underline strikethrough fontsize forecolor backcolor subscript superscript | lineheight alignleft aligncenter alignright alignjustify outdent indent',
		'bullist numlist link image media emoticons table insertdatetime blockquote codesample charmap | pagebreak anchor fullscreen | undo redo searchreplace help',
	],
	fontFamily =
		'微软雅黑=Microsoft YaHei,Helvetica Neue;PingFang SC;sans-serif;苹果苹方=PingFang SC,Microsoft YaHei,sans-serif;宋体=simsun;serifsans-serif;Terminal=terminal;monaco;Times New Roman=times new roman;times';

import editor from '@tinymce/tinymce-vue';
import mirror from './mirror.vue';
export default {
	name: 'vxe-editor',
	components: { editor, mirror },
	props: {
		data: { type: Object, require: true },
		field: { type: String, require: true },
		props: { type: Object, default: {} },
	},
	data() {
		const baseUrl = () => {
			var baseUrl = (this.props || {}).baseUrl || '';
			if (baseUrl == '') console.error('Invalid Url: The value of props.baseUrl can not be null!');
			return baseUrl;
		};
		let url = !!this._config.absolutPath && process.env.NODE_ENV == 'production' ? '/' + this.TrimEnd(this.TrimStart(this._config.absolutPath, '/'), '/') : '';
		return {
			jsSrc: `${url}/static/tinymce/tinymce.min.js`,
			code: false,
			module: { editor: { width: '100%' }, mirror: { width: '0%', left: '0%' }, splitBtnChecked: false, switchBtnChecked: false },
			config: {
				height: '100%',
				language: 'zh_CN',
				document_base_url: baseUrl(),
				// 操作设置配置
				menubar: menubar, // 默认菜单栏
				toolbar: toolbar, // 自定义工具栏
				plugins: plugins, // 插件定义
				removed_menuitems: 'code', // 移除默认菜单栏中的功能，配合自定义工具类进行使用
				contextmenu: 'copy | undo redo searchreplace | link image table spellchecker', // 右键菜单
				contextmenu_never_use_native: true,
				quickbars_insert_toolbar: '', // 编辑时，接收到enter事件时，显示快速插入的按钮
				quickbars_selection_toolbar: '', // 编辑时，接收选择的事件，显示操作按钮
				// 界面配置
				placeholder: 'input content...',
				font_formats: fontFamily, // 字体
				resize: false, // 是否允许用户缩放界面
				branding: false, //是否显示tinymce标识
				elementpath: false, //是否显示底部的元素路径
				statusbar: true, // 是否显示状态栏
				paste_as_text: true, // 是否允许粘贴文本
				paste_data_images: true, // 是否允许粘贴图片
				imagetools_toolbar: true, //图片编辑工具
				insertdatetime_formats: [
					'%H:%M:%S',
					'%H时%M分%S秒',
					'%Y-%m-%d',
					'%Y年%m月%d日',
					'%Y-%m-%d %H:%M:%S',
					'%Y年%m月%d日 %H时%M分%S秒',
					'%A',
					'@DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")',
				], //时间格式化处理
				default_link_target: '_blank',
				forced_root_block: 'div', //强制使用div包裹区域

				// 静态资源处理
				image_uploadtab: false, //图片对话框显示上传标签页
				image_advtab: true, //图片高级设置

				//URL处理
				allow_script_urls: true, // 是否允许url使用js
				relative_urls: true, // 转换所有url为相对路径
				browser_spellcheck: true, // 拼写检查
			},
		};
	},
	computed: {
		splitBtnTooltip() {
			return this.module.splitBtnChecked ? '关闭代码分屏' : '打开代码分屏';
		},
		switchBtnTooltip() {
			return this.module.switchBtnChecked ? '关闭代码编辑器' : '打开代码编辑器';
		},
		height() {
			var height = (this.props || {}).height || 640;
			if (typeof height == 'number') height = `${height / 16}rem`;
			else if (typeof height == 'string' && (height.includes('%') || height.includes('px') || height.includes('rem') || height.includes('em'))) height = height;
			else height = '40rem';
			return height;
		},
	},
	methods: {
		SplitBtnClick() {
			this.data[this.field] = this.$codeFormat(this.data[this.field]);
			this.module.switchBtnChecked = false;
			this.module.editor.width = this.module.splitBtnChecked ? '50%' : '100%';
			this.module.mirror.width = this.module.splitBtnChecked ? '50%' : '0%';
			this.module.mirror.left = this.module.splitBtnChecked ? '50%' : '0%';
		},
		SwitchBtnClick() {
			this.data[this.field] = this.$codeFormat(this.data[this.field]);
			this.module.splitBtnChecked = false;
			this.module.editor.width = this.module.switchBtnChecked ? '0%' : '100%';
			this.module.mirror.width = this.module.switchBtnChecked ? '100%' : '0%';
			this.module.mirror.left = '0%';
		},
	},
};
</script>

<style>
.vxe-tiny-editor {
	height: v-bind(height);
	margin-bottom: 1.875rem;
}
.half {
	height: v-bind(height);
	display: inline-block;
	position: absolute;
}
.code-btn {
	float: right;
	right: 12px;
	z-index: 2;
	top: 5px;
	font-size: 1rem !important;
	font-weight: bolder;
	border-radius: 6px;
	margin-right: 0.3125rem;
	padding: 0 0.3125rem;
	height: 1.5rem;
}
.code-btn-bg {
	background-color: #409eff;
}
.editor-title {
	height: 28px;
	padding: 0 20px;
	font-size: 18px;
	font-weight: bolder;
	background: linear-gradient(to right top, #74e2fb, white);
	border-radius: 0.3125rem;
}
.editor-title .vxe-checkbox--icon {
	display: none;
}
.editor-title i.fa {
	font-size: 1.125rem !important;
	width: 100%;
	margin-right: 6px;
	padding: 0;
}
</style>

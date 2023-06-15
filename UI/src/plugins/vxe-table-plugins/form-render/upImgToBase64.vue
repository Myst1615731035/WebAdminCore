<template>
	<div class="upBase64-container">
		<div v-if="Array.isArray(data[field]) || config.multiple" class="upBase64-main">
			<div class="upload-img-to-base64" v-for="(t, i) in data[field]" :key="i">
				<img :src="t" width="148" height="148" />
				<div class="upload-bg-img">
					<div class="upBase64-btn">
						<span @click="remove(i)">
							<i class="fas fa-trash-alt" />
						</span>
					</div>
				</div>
			</div>
			<label class="upload-img-to-base64" :for="id" v-show="!(config.limit !== 0 && config.limit == (data[field] || []).length)" @drop="dropEvent" @dragover="allowDrop">
				<el-icon style="font-size: 30px"><Plus /></el-icon>
			</label>
		</div>
		<div v-else class="upBase64-main single">
			<div v-show="!!data[field]" class="upload-img-to-base64">
				<img :src="data[field]" width="148" height="148" />
				<div class="upload-bg-img">
					<div class="upBase64-btn">
						<span @click="data[field] = ''">
							<i class="fas fa-trash-alt" />
						</span>
					</div>
				</div>
			</div>
			<label class="upload-img-to-base64" :for="id" v-show="!!!data[field]" @drop="dropEvent" @dragover="allowDrop">
				<el-icon style="font-size: 30px"><Plus /></el-icon>
			</label>
		</div>
		<input ref="vueUploadImg" :disabled="config.disabled" type="file" :id="id" :accept="config.accept" hidden @change="change" :multiple="config.multiple" />
	</div>
</template>

<script>
export default {
	props: {
		data: { type: Object, require: true },
		field: { type: String, require: true },
		props: { type: Object, default: {} },
	},
	emits: [],
	data() {
		let config = Object.assign(
			{
				multiple: false,
				disabled: false,
				accept: 'image/*',
				maxSize: 1024 * 20,
				limit: 0,
				compress: true,
				quality: 0.8,
				distinct: true,
			},
			this.props
		);
		config.limit = config.multiple ? config.limit : 1;
		config.quality = config.compress ? config.quality : 1;
		return {
			id: 'file' + Math.random().toString(16).slice(-13).replace(/\./g, ''),
			config: config,
		};
	},
	methods: {
		computedSize(size, unit = 0) {
			const units = ['B', 'K', 'M', 'G', 'T', 'P'];
			if (size >= 1024) {
				unit++;
				size = size / 1024;
				return this.computedSize(size, unit);
			}
			return `${size.toFixed()}${units[unit]}`;
		},
		async change(e) {
			e.stopPropagation();
			e.preventDefault();
			this.uploadFile(e.target.files);
			this.$refs.vueUploadImg.value = '';
		},
		dropEvent(e) {
			e.preventDefault();
			e.stopPropagation();
			this.uploadFile(e.dataTransfer.files);
		},
		allowDrop(e) {
			e.preventDefault();
			e.stopPropagation();
		},
		uploadFile(files) {
			let promises = [];
			// 获取文件列表
			if (files.length > this.config.limit) {
				this.$message({ content: `选择文件已超出限制上传的数量，请重新上传`, status: 'warning' });
				return;
			}
			// 判断文件大小
			for (var i = 0; i < files.length; i++) {
				promises.push(this.readFile(files[i]));
			}
			Promise.all(promises).then((res) => {
				var alert = false;
				res.forEach((t) => {
					if (!!this.config.maxSize) {
						if (this.getImgSize(t) <= this.config.maxSize) this.add(t);
						else alert = true;
					} else this.add(t);
				});
				if (alert) this.$message({ content: `文件限制在${this.computedSize(this.config.maxSize)}内，部分文件已超出，无法上传`, status: 'warning' });
			});
		},
		readFile(file) {
			return new Promise((next, fail) => {
				const reader = new FileReader();
				// 文件读取方法
				reader.onload = (res) => {
					const fileResult = res.target.result;
					if (this.config.compress) this.compressImg(file, fileResult).then((res) => next(res));
					else next(fileResult);
				};
				// 读取文件
				reader.readAsDataURL(file);
			});
		},
		compressImg(file, fileResult) {
			return new Promise((next, fail) => {
				const img = new Image();
				let canvas = document.createElement('canvas'),
					ctx = canvas.getContext('2d');
				img.onload = () => {
					const w = img.width;
					const h = img.height;
					canvas.setAttribute('width', w);
					canvas.setAttribute('height', h);
					ctx.drawImage(img, 0, 0, w, h);
					const base64 = canvas.toDataURL(file.type, this.config.quality);
					next(base64);
				};
				img.src = fileResult;
			});
		},
		getImgSize(str) {
			var strLength = str.length;
			return parseInt(strLength - (strLength / 8) * 2);
		},
		remove(index) {
			var files = this.data[this.field];
			if (!!files) files.splice(index, 1);
		},
		add(img) {
			// 如果目标值为空,先赋值
			if (!!!this.data[this.field]) {
				if (Array.isArray(this.data[this.field]) || this.config.multiple) this.data[this.field] = [];
				else this.data[this.field] = '';
			}
			if (Array.isArray(this.data[this.field]) || this.config.multiple) this.data[this.field].push(img);
			else this.data[this.field] = img;
		},
	},
};
</script>

<style>
.upBase64-container {
	min-width: 9.25rem;
	height: 9.25rem;
	min-height: 9.25rem;
}
.upBase64-main {
	width: 100%;
	min-width: 100%;
	min-height: 100%;
}
.upload-img-to-base64 {
	position: relative;
	width: 9.25rem;
	height: 9.25rem;
	border-color: #fafafa;
	border-radius: 6px;
	box-sizing: border-box;
	border: 1px dashed #cdd0d6;
	cursor: pointer;
	vertical-align: top;
	display: inline-flex;
	justify-content: center;
	align-items: center;
	margin-right: 1rem;
}
.upload-img-to-base64 img {
	width: 100%;
	height: 100%;
	object-fit: contain;
}
.upload-bg-img {
	position: absolute;
	top: 0;
	left: 0;
	width: 100%;
	height: 100%;
	background: rgba(0, 0, 0, 0.3);
	display: none;
}
.upload-img-to-base64:hover .upload-bg-img {
	display: block;
}
.upBase64-btn {
	width: 100%;
	height: 1.875rem;
	margin: 3.625rem 0;
	text-align: center;
}
.upBase64-btn svg {
	display: inline-block;
	font-size: 1.25rem;
	color: white;
}
</style>

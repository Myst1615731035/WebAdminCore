<template>
	<el-row v-if="Array.isArray(data[field]) && data[field].length > 0" v-for="(t, i) in data[field]" :key="i" :gutter="20">
		<el-col :span="20">
			<vxe-input v-model="data[field][i]" size="medium"></vxe-input>
		</el-col>
		<el-col :span="4">
			<vxe-button v-if="i == data[field].length - 1" icon="fas fa-plus" @click="add"></vxe-button>
			<vxe-button v-if="data[field].length > 1" icon="fas fa-minus" @click="del(i)"></vxe-button>
		</el-col>
	</el-row>
	<vxe-button v-else icon="fas fa-plus" @click="add"></vxe-button>
</template>

<script>
export default {
	props: {
		data: { type: Object, require: true },
		field: { type: String, require: true },
		props: { type: Object, default: {} }
	},
	emits: [],
	methods: {
		async add() {
			if (!!this.data[this.field]) {
				if (Array.isArray(this.data[this.field])) this.data[this.field].push('');
			} else this.data[this.field] = [''];
		},
		async del(index) {
			if (Array.isArray(this.data[this.field])) this.data[this.field].splice(index, 1);
		}
	}
};
</script>

<style>
.el-row {
	margin-top: 0.25rem;
	margin-bottom: 0.25rem;
}
</style>

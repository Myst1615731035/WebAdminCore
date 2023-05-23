<template>
	<vxe-modal ref="modal" v-model="modalShow" v-bind="modalOption" @show="open" @confirm="confirm" @close="close" :before-hide-method="beforeHideMethod">
		<el-tree ref="navtree" v-bind="treeConfig" @node-drag-end="nodeDragEnd"></el-tree>
	</vxe-modal>
</template>

<script>
const GetAllNode = (tree, res = []) => {
	if (!!tree && tree.length > 0 && Array.isArray(tree)) {
		tree.forEach(t => {
			res.push({ Id: t.Id, Pid: t.Pid, Name: t.Name, Path: t.Path, Sort: t.Sort });
			GetAllNode(t.Children, res);
		});
	}
	return res;
};
export default {
	props: ['show'],
	data() {
		return {
			modalShow: this.show,
			modalOption: {
				title: '导航排序 - 拖动排序',
				type: 'confirm',
				showFooter: true,
				width: window.innerWidth * 0.6,
				height: window.innerHeight * 0.9,
				confirmButtonText: '保存'
			},
			treeConfig: {
				data: [],
				draggable: true,
				indent: 30,
				defaultExpandAll: true,
				allowDrop: () => true,
				allowDrag: () => true,
				nodeKey: 'Id',
				props: { label: 'Name', children: 'Children' }
			}
		};
	},
	watch: {
		modalShow() {
			this.$emit('update:show', this.modalShow);
		}
	},
	methods: {
		async nodeDragEnd(dragNode, targetNode, position, event) {
			if (position == 'before' || position == 'after') dragNode.data.Pid = targetNode.data.Pid;
			if (position == 'inner') dragNode.data.Pid = targetNode.data.Id;
			if (position != 'none') {
				var parent = this.$refs.navtree.getNode(dragNode.data.Pid);
				if (!!parent && !!parent.data && !!parent.data.Children) parent.data.Children.forEach((t, i) => (t.Sort = i * 10));
				else this.treeConfig.data.forEach((t, i) => (t.Sort = i * 10));
			}
		},
		async open() {
			this.$post(this.$store.state.serverApi.permission.list).then(res => {
				if (res.success) this.treeConfig.data = res.data;
			});
		},
		async confirm() {
			var list = GetAllNode(this.treeConfig.data);
			this.$post(this.serverApi.permission.saveSort, list).then(res => {
				if (res.success) {
					this.$parent.updateRow();
					this.$refs.modal.close();
				} else this.$refs.modal.beforeHideMethod(true);
				this.$alertRes(res);
			});
		},
		async close() {}
	}
};
</script>

<style></style>

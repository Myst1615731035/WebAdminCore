/* 
	添加自定义渲染，请参考官方示例
	https://vxetable.cn/v4/#/table/renderer/api
 */
import { h } from 'vue';
import VXETable from 'vxe-table';

/* 
	表单渲染使用的组件参数处理方法
 */
const FormItemRender = (node, renderOpts, params, defaultProps = null, defaultEvents = null) => {
	var props = Object.assign(defaultProps || {}, (renderOpts || {}).props || {});
	var events = Object.assign(defaultEvents || {}, (renderOpts || {}).events || {});
	var res = { data: (params || {}).data || {}, field: (params || {}).property || '', props };
	if (!!node && !!node.emits && node.emits.length > 0) node.emits.forEach(t => (res[`on${FirstToUpper(t)}`] = events[t]));
	return res;
};

/**
 * 扩展插件规范：
 * 组件属性所需参数(命名统一)：
 * v-model：data, field
 * v-bind：props,
 * 其他参数自定
 */

const formRenders = import.meta.globEager('./form-render/*.vue');
// const cellRenders = import.meta.globEager('./cell-render/*.vue');
let modules = {};
for (var i in formRenders) {
	modules[`$${i.match(/[^/]+(?!.*\/)+(?=.vue)/gi)[0]}`] = {
		renderItemContent(renderOpts, params) {
			return h(modules[renderOpts.name].default, FormItemRender(modules[renderOpts.name].default, renderOpts, params));
		}
	};
	modules[`$${i.match(/[^/]+(?!.*\/)+(?=.vue)/gi)[0]}`].default = formRenders[i].default;
}
VXETable.renderer.mixin(modules);

// import radioBtnGroup from './form-render/radioBtnGroup.vue';
// VXETable.renderer.add('$radioBtnGroup', {
// 	renderItemContent(renderOpts, params) {
// 		return h(radioBtnGroup, FormItemRender(radioBtnGroup, renderOpts, params));
// 	}
// });

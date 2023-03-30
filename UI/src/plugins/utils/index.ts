// axios
import axios from './http/index';
// ExcelHelper
import excel from './excel/index';
// 全局函数
import common from './common/index';
export default {
	install: app =>
		app
			.use(axios)
			.use(excel)
			.use(common)
};

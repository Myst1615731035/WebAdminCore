/* 
	excel导出与导入，请参考该文档
	https://github.com/exceljs/exceljs/blob/master/README_zh.md
 */

import * as exceljs from 'exceljs';
import * as fs from 'file-saver';
const { Workbook } = exceljs;
const JsonToExcel = (data, columns = null) => {
	if (IsNotEmpty(data) && data.length > 0 && Array.isArray(data) && typeof data[0] == 'object') {
		const workbook = new Workbook();
		const sheet = workbook.addWorksheet('Sheet1');
		if (!!columns) {
			columns = [];
			Object.keys(data[0]).forEach((t) => {
				columns.push({ header: t, key: t });
			});
		}
		sheet.columns = columns;
		sheet.addRows(data);
		workbook.xlsx.writeBuffer((res) => {
			const blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8' });
			fs.saveAs(blob, 'json.xlsx');
		});
	} else console.log('data is empty, can not export to excel;');
};

export default {
	install: (app) => {
		app.config.globalProperties.JsonToExcel = JsonToExcel;
	},
};

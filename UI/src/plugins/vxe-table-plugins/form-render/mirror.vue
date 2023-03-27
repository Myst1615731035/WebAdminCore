<template>
	<codemirror class="codemirror" ref="cm" v-model="data[field]" v-bind="config" :extensions="extensions" :phrases="phrases"></codemirror>
</template>
<script>
// 组件包
import { Codemirror } from 'vue-codemirror';
// 语言包
import { html } from '@codemirror/lang-html';
import { json } from '@codemirror/lang-json';
import { javascript } from '@codemirror/lang-javascript';
import { css } from '@codemirror/lang-css';
import { sql } from '@codemirror/lang-sql';
import { xml } from '@codemirror/lang-xml';
import { oneDark } from '@codemirror/theme-one-dark';
// 功能包
import '@codemirror/search';
import '@codemirror/lint';
import '@codemirror/state';
import '@codemirror/autocomplete';
import '@codemirror/language';
import '@codemirror/view';
import '@codemirror/commands';
const languages = { html: html(), css: css(), javascript: javascript(), json: json(), sql: sql(), xml: xml() };
const themes = { oneDark };
const germanPhrases = {
	'Control character': 'Steuerzeichen',
	'Selection deleted': 'Auswahl gelöscht',
	'Folded lines': 'Eingeklappte Zeilen',
	'Unfolded lines': 'Ausgeklappte Zeilen',
	to: 'bis',
	'folded code': 'eingeklappter Code',
	unfold: 'ausklappen',
	'Fold line': 'Zeile einklappen',
	'Unfold line': 'Zeile ausklappen',
	'Go to line': 'Springe zu Zeile',
	go: 'OK',
	Find: 'Suchen',
	Replace: 'Ersetzen',
	next: 'nächste',
	previous: 'vorherige',
	all: 'alle',
	'match case': 'groß/klein beachten',
	'by word': 'ganze Wörter',
	replace: 'ersetzen',
	'replace all': 'alle ersetzen',
	close: 'schließen',
	'current match': 'aktueller Treffer',
	'replaced $ matches': '$ Treffer ersetzt',
	'replaced match on line $': 'Treffer on Zeile $ ersetzt',
	'on line': 'auf Zeile',
	Completions: 'Vervollständigungen',
	Diagnostics: 'Diagnosen',
	'No diagnostics': 'Keine Diagnosen'
};
export default {
	name: 'vxe-codemirro',
	props: {
		// 表单数据
		data: { type: Object, required: true, default: {} },
		// 字段
		field: { type: String, required: true },
		// codemirror配置
		props: { type: Object, required: true, default: {} }
	},
	components: { Codemirror },
	data() {
		return {
			phrases: germanPhrases
		};
	},
	computed: {
		height() {
			var height = this.props.height;
			if (typeof height == 'number') height = `${height / 16}rem`;
			else if (typeof height == 'string' && (height.includes('%') || height.includes('px') || height.includes('rem') || height.includes('em'))) height = height;
			else height = '30rem';
			return height;
		},
		extensions() {
			const result = [];
			result.push(languages[this.config.language]);
			if (!!themes[this.config.theme]) result.push(themes[this.config.theme]);
			return result;
		},
		config() {
			var defaultConfig = { disabled: false, indentWithTab: true, tabSize: 4, autofocus: true, placeholder: 'input...', language: 'html', theme: '' };
			return Object.assign(defaultConfig, this.props);
		}
	}
};
</script>

<style>
.codemirror {
	display: block !important;
	border: 1px solid #eee;
}
.codemirror .cm-editor {
	height: v-bind(height);
	max-height: v-bind(height);
}
</style>

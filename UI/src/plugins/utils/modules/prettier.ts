import prettier from 'prettier';
import parserBabel from 'prettier/parser-babel';
import parserHtml from 'prettier/parser-html';
const defaultOptions = {
	arrowParens: "always",
	bracketSameLine: true,
	bracketSpacing: true,
	embeddedLanguageFormatting: "auto",
	htmlWhitespaceSensitivity: "css",
	insertPragma: false,
	jsxSingleQuote: false,
	printWidth: 160,
	proseWrap: "preserve",
	quoteProps: "preserve",
	requirePragma: false,
	semi: true,
	singleAttributePerLine: false,
	singleQuote: true,
	tabWidth: 4,
	trailingComma: "es5",
	useTabs: false,
	vueIndentScriptAndStyle: false,
	parser: 'html',
	plugins: [parserBabel, parserHtml]
};
export default {
	install: (app) => {
		app.config.globalProperties.$codeFormat = (text, options = null) => prettier.format(text, Object.assign(defaultOptions, options || {}));
	}
};

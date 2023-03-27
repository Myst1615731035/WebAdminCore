// 引入外部可修改的配置文件
import _config from './public/Settings.js';
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import AutoImport from 'unplugin-auto-import/vite';
import Components from 'unplugin-vue-components/vite';
import { ElementPlusResolver } from 'unplugin-vue-components/resolvers';
import { createStyleImportPlugin, VxeTableResolve } from 'vite-plugin-style-import';
// import VitePluginCompression from 'vite-plugin-compression';
// https://vitejs.dev/config/
export default defineConfig(({ command, mode }) => {
	const productBase = '/adminpage/';
	let configs = {
		plugins: [
			vue(),
			AutoImport({ resolvers: [ElementPlusResolver()] }),
			Components({ resolvers: [ElementPlusResolver()] }),
			createStyleImportPlugin({ resolves: [VxeTableResolve()] })
			// VitePluginCompression()
		],
		// 添加此项，在组件内才可使用process.env.NODE_ENV
		define: { 'process.env': {} }
	};
	switch (command) {
		case 'build':
			configs = Object.assign(configs, {
				base: _config.absolutPath,
				build: {
					outDir: '../MainCore/wwwroot',
					emptyOutDir: true,
					rollupOptions: {
						output: {
							manualChunks(id) {
								if (id.includes('node_modules'))
									return id
										.toString()
										.split('node_modules/')[1]
										.split('/')[0]
										.toString();
							},
							assetFileNames: `assets/[name][extname]`,
							chunkFileNames: `js/[name].js`
						}
					}
				}
			});
			// 生产环境
			break;
		default:
			// 开发环境
			configs = Object.assign(configs, {
				server: {
					host: '127.0.0.1',
					port: 10012,
					strictPort: true,
					https: false,
					open: true,
					proxy: {
						'/api': { target: 'http://127.0.0.1:10011', ws: true, changeOrigin: true },
						'/template/': { target: 'http://127.0.0.1:10011', ws: true, changeOrigin: true },
						'/export/': { target: 'http://127.0.0.1:10011', ws: true, changeOrigin: true },
						'/is4api': { target: 'http://127.0.0.1:9090', ws: true, changeOrigin: true }
					}
				}
			});
			break;
	}
	return configs;
});

export default {
	install: app => {
		app.config.globalProperties.Trim = Trim;
		app.config.globalProperties.TrimStart = TrimStart;
		app.config.globalProperties.TrimEnd = TrimEnd;
		app.config.globalProperties.FirstToUpper = FirstToUpper;
	}
};

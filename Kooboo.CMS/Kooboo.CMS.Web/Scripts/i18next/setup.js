jqueryI18next.init(i18nextInstance, $, {
    tName: 't', // --> appends $.t = i18next.t 
    i18nName: 'i18n', // --> appends $.i18n = i18next 
    handleName: 'localize', // --> appends $(selector).localize(opts); 
    selectorAttr: 'data-i18n', // selector for translating elements 
    targetAttr: 'i18n-target', // data-() attribute to grab target element to translate (if diffrent then itself) 
    optionsAttr: 'i18n-options', // data-() attribute that contains options, will load/set if useOptionsAttr = true 
    useOptionsAttr: false, // see optionsAttr 
    parseDefaultValueFromContent: true // parses default values from content ele.val or ele.text 
});
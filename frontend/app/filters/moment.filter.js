(function(){
  angular
    .module('CRM')
    .filter('moment', MomentFilter);

  function MomentFilter(){
    return function(date, format){
      if(!date) return "";
      if(typeof(date) === "string") date = moment.utc(date);
      if(!format) format = "ll";
      return date.format(format);
    };
  }
})();

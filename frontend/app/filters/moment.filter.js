(function(){
  angular
    .module('CRM')
    .filter('moment', MomentFilter);

  function MomentFilter(){
    return function(date, format){
      if(!format) format = "ll";
      return date.format(format);
    };
  }
})();

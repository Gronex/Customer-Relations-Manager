(function(){
  angular
    .module('CRM')
    .filter('moment', MomentFilter);

  function MomentFilter(){
    return function(date, format){
      if(typeof(date) === "string") date = moment(date);
      if(!format) format = "ll";
      return date.format(format);
    };
  }
})();

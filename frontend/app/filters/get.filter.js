(function(){
  angular
    .module('CRM')
    .filter('get', GetFilter);

  function GetFilter(){
    return function(object, selector){
      return _.get(object, selector);
    };
  }
})();

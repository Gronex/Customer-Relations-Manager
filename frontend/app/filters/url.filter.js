(function(){
  angular
    .module('CRM')
    .filter('url', UrlFilter);

  function UrlFilter(){
    return function(link){
      if(!link) return "";
      if(link.match(/:\/\//)) return link;
      link = "http://" + link;

      return link;
    };
  }
})();

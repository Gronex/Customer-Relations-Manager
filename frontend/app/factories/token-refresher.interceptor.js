(function(){
  'use strict';

  angular
    .module("CRM")
    .factory("tokenRefresher", factory);

  factory.$inject = ["$injector"];
  function factory($injector){
    return {
      request: request
    };

    function request(config){
      if(config.url == "api/token") return config;
      var auth = $injector.get("authorization");
      if(auth.isExpired())
        return auth.refresh()
        .then(function(token){
          config.headers["Authorization"] = token;
          return config;
        },function(){return config;});
      return config;
    }
  }
})();

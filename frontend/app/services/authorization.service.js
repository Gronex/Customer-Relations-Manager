(function(){
  angular
    .module('CRM')
    .factory('authorization', Authorization);

  Authorization.$inject = ['localStorageService', '$http', '$log'];
  function Authorization(localStorageService, $http, $log) {
    return {
      configToken: configToken,
      configUser: configUser,
      getUser: getUser,
      logout: logout
    };

    function configToken(token) {
      if(!token){
        token = localStorageService.get("token");
        if(token === undefined) return false;
        $log.info("User loaded");
      }
      localStorageService.set("token", token);
      $http.defaults.headers.common.Authorization = "Bearer " + token;
      return token;
    }

    function configUser(args) {
      var user = {
        name: args.name,
        email: args.userName
      };
      localStorageService.set("user", user);
      return user;
    }

    function getUser() {
      var user = localStorageService.get("user");
      return user;
    }

    function logout() {
      localStorageService.remove("user");
      localStorageService.remove("token");
    }

  }
})();

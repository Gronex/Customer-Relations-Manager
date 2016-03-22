(function(){
  angular
    .module('CRM')
    .factory('authorization', Authorization);

  Authorization.$inject = ['$rootScope', 'localStorageService', '$http', '$log'];
  function Authorization($scope, localStorageService, $http, $log) {

    var user = null;
    var subscribed = [];

    return {
      login: login,
      getUser: getUser,
      logout: logout,
      subscribe: subscribe
    };
    function login(userName, password) {
      if(userName == undefined){
        configToken();
        onChange();
        return getUser();
      }
      return $http({
        method: 'POST',
        url: "api/token",
        data: $.param({
          "grant_type": "password",
          "username": userName,
          "password": password
        }),
        headers: {'Content-Type': 'application/x-www-form-urlencoded'}
      }).then(function (response) {
        configToken(response.data["access_token"]);
        return configUser(response.data);
      });
    }

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
      user = {
        name: args.name,
        email: args.userName,
        roles: args.roles.split(",")
      };
      localStorageService.set("user", user);
      onChange();
      return user;
    }

    function getUser() {
      if(!!user) return user;
      user = localStorageService.get("user");
      return user;
    }

    function logout() {
      localStorageService.remove("user");
      localStorageService.remove("token");
      user = null;
      onChange();
    }



    function subscribe(fun){
      if(typeof(fun) !== "function"){
        throw "argument must be a function";
      };
      subscribed.push(fun);
    }

    function onChange(){
      $log.info("User changed!!");
      $log.info(subscribed.length);
      _.map(subscribed, function(fun){ fun(user); });
    }
  }
})();

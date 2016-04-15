(function(){
  'use strict';

  angular
    .module('CRM')
    .factory('authorization', Authorization);

  Authorization.$inject = ['$rootScope', 'localStorageService', '$http', '$log', '$q'];
  function Authorization($scope, localStorageService, $http, $log, $q) {

    var user = null;
    var subscribed = [];

    return {
      login: login,
      getUser: getUser,
      logout: logout,
      subscribe: subscribe,
      isExpired: isExpired,
      refresh: refresh
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
          "password": password,
          "client_id": "angular"
        }),
        headers: {'Content-Type': 'application/x-www-form-urlencoded'}
      }).then(function (response) {
        configToken(response.data);
        return configUser(response.data);
      });
    }

    function configToken(token) {
      if(!token){
        token = localStorageService.get("token");
        if(!token) return false;
      }else {
        token = {
          accessToken: token["access_token"],
          expires: moment(new Date(token[".expires"])).format(),
          refreshToken: token["refresh_token"]
        };
      }
      localStorageService.set("token", token);
      $http.defaults.headers.common.Authorization = "Bearer " + token.accessToken;
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
      if(user) return user;
      user = localStorageService.get("user");
      $log.info("User loaded");
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
      }
      subscribed.push(fun);
    }

    function onChange(){
      _.map(subscribed, function(fun){ fun(user); });
    }

    function isExpired(){
      var token = localStorageService.get("token");
      if(!token) return true;
      // Give one minute buffer
      var expires = moment(token.expires).subtract(1, "minute");
      return expires.isSameOrBefore();
    }

    function refresh(){
      var token = localStorageService.get("token");
      if(!token) return $q.reject();
      var refreshToken = token.refreshToken;

      return $http({
        method: 'POST',
        url: "api/token",
        data: $.param({
          "grant_type": "refresh_token",
          "client_id": "angular",
          "refresh_token": refreshToken
        }),
        headers: {'Content-Type': 'application/x-www-form-urlencoded'}
      }).then(function (response) {
        var token = configToken(response.data);
        configUser(response.data);
        onChange();
        return token;
      });
    }
  }
})();

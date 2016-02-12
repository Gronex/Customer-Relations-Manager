(function(){
  angular
    .module('CRM')
    .factory('dataservice', dataservice);

  dataservice.$inject = ["$http", '$q', '$log', 'localStorageService'];
  function dataservice($http, $q, $log, localStorageService) {

    return {
      configToken: configToken,

      login: login,

      users: createResource("/api/users"),
      userGroups: createResource("/api/usergroups")
    };

    function configToken(token) {
      if(!token){
        token = localStorageService.get("token");
        if(token === undefined) return false;
        $log.info("User loaded");
      }
      $http.defaults.headers.common.Authorization = "Bearer " + token;
      return true;
    }

    function login(userName, password) {
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
        localStorageService.set("token", response.data["access_token"]);
        configToken(response.data["access_token"]);
      });
    }

    function createResource(url) {
      return {
        getAll: getAll,
        getById: getById,
        create: create,
        update: update,
        remove: remove
      };

      function getAll() {
        return $http.get(url)
          .then(returnData, handleError);
      }

      function getById(id) {
        return $http.get(url + "/" + id)
          .then(returnData, handleError);
      }

      function create(data) {
        return $http.post(url, data)
          .then(returnData, handleError);
      }

      function update(id, data) {
        return $http.put(url + "/" + id, data)
          .then(returnData, handleError);
      }

      function remove(id) {
        return $http.delete(url + "/" + id)
          .then(returnData, handleError);
      }
    }

    function handleError(err) {
      $log.error("XHR Failed with code: '" + err.status + "' on '" + err.config.method + " " + err.config.url + "'");
      return $q.reject(err);
    }

    function returnData(response) {
      return response.data;
    }
  }
})();

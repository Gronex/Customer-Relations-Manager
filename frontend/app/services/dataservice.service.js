(function(){
  angular
    .module('CRM')
    .factory('dataservice', dataservice);

  dataservice.$inject = ["$http", '$q', '$log', 'localStorageService'];
  function dataservice($http, $q, $log, localStorageService) {

    return {
      configToken: configToken,

      login: login,

      getUsers: getUsers,
      getUser: getUser,
      updateUser: updateUser,
      createUser: createUser,
      deleteUser: deleteUser,

      getUserGroups: getUserGroups,
      createUserGroup: createUserGroup,
      deleteUserGroup: deleteUserGroup,
      updateUserGroup: updateUserGroup
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

    function getUsers() {
      return $http.get('/api/users')
        .then(returnData, handleError);
    }

    function getUser(id) {
      return $http.get('/api/users/' + id)
        .then(returnData, handleError);
    }

    function updateUser(id, user) {
      return $http.put('/api/users/' + id, user)
        .then(returnData, handleError);
    }

    function createUser(id, user) {
      return $http.post('/api/users', user)
        .then(returnData, handleError);
    }

    function deleteUser(id) {
      return $http.delete('/api/users/'+id)
        .then(returnData, handleError);
    }

    function getUserGroups() {
      return $http.get('/api/usergroups')
        .then(returnData, handleError);
    }

    function createUserGroup(group) {
      return $http.post('/api/usergroups', group)
        .then(returnData, handleError);
    }

    function deleteUserGroup(id) {
      return $http.delete('/api/usergroups/' + id)
        .then(returnData, handleError);
    }

    function updateUserGroup(id, group) {
      return $http.put('/api/usergroups/' + id, group)
        .then(returnData, handleError);
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

(function(){
  angular
    .module('CRM')
    .factory('dataservice', dataservice);

  dataservice.$inject = ["$http", '$log', 'localStorageService'];
  function dataservice($http, $log, localStorageService) {

    return {
      configToken: configToken,

      login: login,

      getUsers: getUsers,
      getUser: getUser,
      updateUser: updateUser,
      createUser: createUser
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
        .then(getUsersComplete)
        .catch(getUsersFailed);

      function getUsersComplete(response) {
        return response.data;
      }

      function getUsersFailed(error) {
        $log.error('XHR Failed for getUsers.' + error.data);
      }
    }

    function getUser(id) {
      return $http.get('/api/users/' + id)
        .then(getUserComplete)
        .catch(getUserFailed);

      function getUserComplete(response) {
        return response.data;
      }

      function getUserFailed(error) {
        $log.error(error.data);
        $log.error('XHR Failed for getUser.' + error.data);
      }
    }

    function updateUser(id, user) {
      return $http.put('/api/users/' + id, user)
        .then(updateUserComplete)
        .catch(updateUserFailed);

      function updateUserComplete(response) {
        return response.data;
      }

      function updateUserFailed(error) {
        $log.error(error.data);
        $log.error('XHR Failed for updateUser.' + error.data);
      }
    }

    function createUser(id, user) {
      return $http.post('/api/users', user)
        .then(createUserComplete)
        .catch(createUserFailed);

      function createUserComplete(response) {
        return response.data;
      }

      function createUserFailed(error) {
        $log.error(error.data);
        $log.error('XHR Failed for createUser.' + error.data);
      }
    }
  }
})();

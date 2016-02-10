(function(){
  angular
    .module('app')
    .factory('dataservice', dataservice);

  dataservice.$inject = ["$http", '$log'];
  function dataservice($http, $log) {
    config();

    return {
      login: login,

      getUsers: getUsers,
      getUser: getUser,
      updateUser: updateUser,
      createUser: createUser
    };

    function login() {

    }

    function config() {
      $http.defaults.headers.common.Authorization = "Bearer l8jK3JrNSqEdcTuPCtXnKrwG3ef4lFg5BZjC9fC8gE4tyu1FcKz_Wz2Vu2a8QTOdtDvwQ7zcbofZgJY4dWLRkVqmWuwQUGwR4ufs6_eXde8heZsZyM-YlIux_Jgnd-xT8GDmU7K039Rw5csZ5ThE4VBbCkIL_r8dDbtZzLpG3Uw0Wm4ocaFqdgJdB0jm01ED8OILbXWnpWwZMgVnI3EM_lpVuLwdinZjNNcby8NtZhnRukACpOnI1DBUv0feGgihDywaGc-_8H2tPa05rMAHED2glGX0bBDxuKLUTFQ00cS2_DAp0zWMkJwd2Y70lmPH3iSCWId0DGy9jrpOPjuYtTZEBMKBTNe8HKx_thGitIHKhVEAhiWLmz9v9a71UxwhbGUZPtxYCroswJVpnwMDc8ArcJ7qYH9Yrn8rnZJ-BKFnjnCo3kRrDn4oFKRw31ekOufUD-R-0Lzt_3deTF_Hb5EqU7HIbbYUjld7lyBsMe3Y5dHd80eK_EGR7srBc6pwN3RoYhX46Cnt9aFu-nEPfJ-m73Mc1PlWqTobTLDVxzhxkmEHqM0o965qOSDBx7QB";
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

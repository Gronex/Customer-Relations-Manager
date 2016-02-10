(function() {
  'use strict';

  angular
    .module('app')
    .controller('Users', Users);

  Users.$inject = ['dataservice', '$log'];
  function Users(dataservice){
    var vm = this;
    vm.users = [];

    activate();

    function activate() {
      getUsers();
    }

    function getUsers() {
      return dataservice.getUsers()
        .then(function (data) {
          vm.users = data;
          return vm.users;
        });
    }

  }
})();

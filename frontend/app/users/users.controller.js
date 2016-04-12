(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Users', Users);

  Users.$inject = ['users', '$state'];
  function Users(users, $state){
    var vm = this;
    vm.users = [];
    vm.itemCount = 0;

    vm.headers = [
      {
        label: "First name",
        selector: "firstName"
      },
      {
        label: "Last name",
        selector: "lastName"
      },
      {
        label: "Email",
        type: "email",
        selector: "email"
      },
      {
        label: "Role",
        selector: "role"
      }
    ];

    vm.getUsers = getUsers;

    activate();

    function activate() {
      vm.users = users.data;
      vm.query = users.query;
      vm.itemCount = users.itemCount;
    }

    function getUsers(sortParam){
      _.merge(vm.query, {orderBy: sortParam});
      $state.go(".", vm.query);
    }
  }
})();

(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Login', Login);

  Login.$inject = ['authorization', '$state'];
  function Login(authorization, $state){
    var vm = this;
    vm.userName = "";
    vm.password = "";

    vm.login = login;

    function login() {
      return authorization.login(vm.userName, vm.password)
        .then(function () {
          $state.go("Home");
        });
    }

  }
})();

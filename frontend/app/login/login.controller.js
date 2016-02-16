(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Login', Login);

  Login.$inject = ['dataservice', 'authorization', '$state'];
  function Login(dataservice, authorization, $state){
    var vm = this;
    vm.userName = "";
    vm.password = "";

    vm.login = login;

    function login() {
      return dataservice.login(vm.userName, vm.password)
        .then(function () {
          $state.go("Home");
        });
    }

  }
})();

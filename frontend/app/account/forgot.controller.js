(function(){
  'use strict';

  angular
    .module("CRM")
    .controller("ForgotPassword", Controller);

  Controller.$inject = ['$stateParams', 'dataservice'];
  function Controller($stateParams, dataservice){
    var vm = this;

    vm.send = send;

    activate();

    function activate(){
      vm.email = $stateParams.email;
    }

    function send(){
      dataservice.account.get({id: "forgotPassword", query: {userName: vm.email}})
        .then(function(){
          vm.done = true;
        });
    }
  }
})();

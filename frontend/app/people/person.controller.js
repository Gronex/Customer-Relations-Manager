(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Person', Person);

  Person.$inject = ['dataservice', '$stateParams', '$state'];

  /* @ngInject */
  function Person(dataservice, $stateParams, $state) {
    var vm = this;

    vm.person = {};

    vm.updateCompany = updateCompany;
    vm.save = save;
    vm.removeFromAll = removeFromAll;

    activate();

    function activate() {
      if($stateParams.id !== "new") vm.editing = true;
      if(vm.editing){
        getPerson();
      }
    }

    function getPerson() {
      dataservice.people
        .getById($stateParams.id)
        .then(function (data) {
          vm.person = data;
        });
    }

    function save() {
      if(vm.editing){
        dataservice.people
        .update($stateParams.id, vm.person)
        .then(function () {
          $state.go("People");
        });
      }
      else {
        dataservice.people
        .create(vm.person)
        .then(function (data) {
          $state.go("Person", {id: data.location});
        });
      }
    }

    function removeFromAll() {
      dataservice.people
        .remove($stateParams)
        .then(function () {
          $state.go("People");
        });
    }

    function updateCompany(company){
      vm.person.companyName = company.name;
      vm.person.companyId = company.id;
    }
  }
})();

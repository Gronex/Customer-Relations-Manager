(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('EditPerson', Person);

  Person.$inject = ['person','dataservice', '$stateParams', '$state'];

  /* @ngInject */
  function Person(person, dataservice, $stateParams, $state) {
    var vm = this;
    vm.updateCompany = updateCompany;
    vm.save = save;
    vm.removeFromAll = removeFromAll;

    activate();

    function activate() {
      if($state.is("People.edit")) vm.editing = true;
      vm.person = person;
    }

    function save() {
      if(vm.editing){
        dataservice.people
        .update($stateParams.id, vm.person)
        .then(function () {
          $state.go("People.list");
        });
      }
      else {
        dataservice.people
        .create(vm.person)
        .then(function (data) {
          $state.go("People.edit", {id: data.location});
        });
      }
    }

    function removeFromAll() {
      dataservice.people
        .remove($stateParams)
        .then(function () {
          vm.person.companyName = undefined;
          vm.person.companyId = undefined;
        });
    }

    function updateCompany(company){
      vm.person.companyName = company.name;
      vm.person.companyId = company.id;
    }
  }
})();

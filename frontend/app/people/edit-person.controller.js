(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('EditPerson', Person);

  Person.$inject = ['person','dataservice', '$stateParams', '$state', 'warning'];

  /* @ngInject */
  function Person(person, dataservice, $stateParams, $state, warning) {
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
        var $save = function(){
          dataservice.people
            .create(vm.person)
            .then(function (data) {
              $state.go("People.edit", {id: data.location});
            });
        };
        dataservice.people.get({query: {find: vm.person.firstName}})
          .then(function(result){
            if(result.data.length < 1){
              $save();
              return;
            }
            warning.warn({
              type: "list",
              list: _.map(result.data, function(d){
                return {
                  text: d.name,
                  action: function(){ $state.go("People.edit", {id: d.id}); }
                };
              }),
              okText: "Keep current",
              header: "Did you mean?"
            }).then($save);
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

(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Departments', Departments);

  Departments.$inject = ['dataservice'];

  /* @ngInject */
  function Departments(dataservice) {
    var vm = this;
    vm.departments = [];
    vm.department = {};

    vm.create = create;
    vm.save = save;
    vm.remove = remove;

    activate();

    function activate() {
      getDepartments();
      resetDepartment();
    }

    function getDepartments() {
      dataservice.departments
        .get()
        .then(function (data) {
          vm.departments = _.orderBy(data, ['name']);
        });
    }

    function create() {
      vm.department.errMsg = undefined;
      console.log(vm.department);
      dataservice.departments
        .create(vm.department)
        .then(function (data) {
          vm.departments.push(data.data);
          resetDepartment();
        }, function (err) { handleError(err, vm.department); });
    }
    function save(department) {
      department.errMsg = undefined;
      if(department.saved || department.saved == undefined) return;
      dataservice.departments
        .update(department.id, department)
        .then(function () {
          department.saved = true;
          resetDepartment();
        }, function (err) { handleError(err, department); });
    }
    function remove(id) {
      dataservice.departments
        .remove(id)
        .then(function () {
          _.remove(vm.departments, function (s) {
            return s.id == id;
          });
        });
    }

    function resetDepartment() {
      vm.departments = _.orderBy(vm.departments, ['name']);
      vm.department = {
        name: ""
      };
    }

    function handleError(err, target) {
      switch (err.status) {
        case 400:

          break;
        case 409:
          target.errMsg = "Group with name: '" + err.data.name + "' already exists";
          break;
        default:
      }
    }
  }
})();

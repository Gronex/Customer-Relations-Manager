(function(){
  'use strict';

  angular
    .module('CRM')
    .directive('crmCommentList', commentList);

  function commentList(){
    var directive = {
      restrict: 'EA',
      templateUrl: 'view/app/components/comments/comment-list.html',
      scope: {
        backendAccess: '=',
        callArgs: '='
      },
      controller: Controller,
      controllerAs: 'vm',
      bindToController: true
    };
    return directive;
  }

  Controller.$inject = [];
  function Controller(){
    var vm = this;
    vm.comments = [];

    vm.send = send;
    vm.pagination = {
      page: 1,
      pageSize: 5
    };
    vm.commentCount = 0;

    vm.getComments = getComments;

    activate();

    function activate(){
      getComments();
    }

    function getComments(){
      var args = _.merge(angular.copy(vm.callArgs), {query: vm.pagination});
      console.log(vm.pagination);
      return vm.backendAccess
        .get(args)
        .then(function(result){
          result.date = _.map(result.data, function(c){
            c.sent = new Date(c.sent);
          });
          vm.comments = result.data;
          vm.commentCount = result.itemCount;
        });
    }

    function send(){
      vm.pagination.page = 1;
      var result = vm.backendAccess
            .create(vm.comment, vm.callArgs)
            .then(getComments);
      vm.comment = "";
      return result;
    }
  }
})();

var gulp = require('gulp');
var del = require('del');

var paths = {
  js: ["scripts/**/*.js", "app/**/*.js"],
  html: ["app/**/*.html"],
  css: ["content/**/*.css"],
  libs: ["bower_components/**", "node_modules/**"],
  index: "index.html"
};

var dest = "../customer_relations_manager";
var fullDest = dest + "/view";

gulp.task("clean", function () {
  return del([fullDest], {force: true});
});

gulp.task("move:js", ["clean"], function () {
  return gulp.src(paths.js, {base: "./"})
    .pipe(gulp.dest(fullDest));
});

gulp.task("move:html", ["clean"], function () {
  return gulp
    .src(paths.html, {base: "./"})
    .pipe(gulp.dest(fullDest));
});

gulp.task("move:index", ["clean"], function () {
  return gulp
    .src(paths.index, {base: "./"})
    .pipe(gulp.dest(dest));
});

gulp.task("move:css", ["clean"], function () {
  return gulp
    .src(paths.css, {base: "./"})
    .pipe(gulp.dest(fullDest));
});

gulp.task("move:libs", ["clean"], function () {
  return gulp
    .src(paths.libs, {base: "./"})
    .pipe(gulp.dest(fullDest));
});

gulp.task("watch", ["build"], function () {
  var everything = paths.js.concat(
    paths.html,
    paths.css,
    paths.libs,
    paths.index
  );

  gulp.watch(everything, ["build"]);
});

gulp.task("build", ["move:js", "move:html", "move:css", "move:libs", "move:index"]);

gulp.task("default", ['clean', 'watch']);

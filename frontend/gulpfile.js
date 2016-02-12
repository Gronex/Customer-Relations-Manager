var gulp = require('gulp');
var del = require('del');

var paths = {
  js: ["scripts/**/*.js", "app/**/*.js"],
  html: ["app/**/*.html"],
  css: ["content/**/*.css"],
  libs: ["bower_components/**"],
  index: "index.html"
};

var dest = "../customer_relations_manager";
var fullDest = dest + "/view";

gulp.task("clean", function () {
  del([fullDest], {force: true});
});

gulp.task("move:js", function () {
  return gulp.src(paths.js, {base: "./"})
    .pipe(gulp.dest(fullDest));
});

gulp.task("move:html", function () {
  return gulp
    .src(paths.html, {base: "./"})
    .pipe(gulp.dest(fullDest));
});

gulp.task("move:index", function () {
  return gulp
    .src(paths.index, {base: "./"})
    .pipe(gulp.dest(dest));
});

gulp.task("move:css", function () {
  return gulp
    .src(paths.css, {base: "./"})
    .pipe(gulp.dest(fullDest));
});

gulp.task("move:libs", function () {
  return gulp
    .src(paths.libs, {base: "./"})
    .pipe(gulp.dest(fullDest));
});

gulp.task("watch", ["clean", "move"], function () {
  var everything = paths.js.concat(
    paths.html,
    paths.css,
    paths.libs,
    paths.index
  );

  gulp.watch(everything, ["move"]);
});

gulp.task("move", ["move:js", "move:html", "move:css", "move:libs", "move:index"]);

gulp.task("default", ["watch"]);

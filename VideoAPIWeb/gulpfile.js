/// <binding />
/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var concat = require('gulp-concat');
var cleaner = require("gulp-rimraf");
var merge = require('merge-stream');

// Dependency Dirs
var deps = {
    "jquery.skillsoft.videorapi": {
        "dist/*": ""
    },
};

gulp.task("clean", function (cb) {
    console.log("Clean all files in vendor folder");

    return gulp.src("js/vendor/*", { read: false }).pipe(cleaner());
});


gulp.task("default", function () {

    var streams = [];

    for (var prop in deps) {
        console.log("Prepping Scripts for: " + prop);
        for (var itemProp in deps[prop]) {
            streams.push(gulp.src("node_modules/" + prop + "/" + itemProp)
                .pipe(gulp.dest("js/vendor/" + prop + "/" + deps[prop][itemProp])));
        }
    }

    return merge(streams);

});

function runJsTests() {
    runSingleTest("testSameStringsSingleton");
    runSingleTest("testSameStringsMultichars");
    runSingleTest("testEmpty");
    runSingleTest("testLineMatching");
    runSingleTest("testLengthOneDelete");
    runSingleTest("testLengthOneInsert");
    runSingleTest("TestMoreDiagonalPreferable");
    runSingleTest("testLongInsert");
    runSingleTest("testLongDelete");
    runSingleTest("testOptimality");

    runSingleTest("testEmptyDescriptors");
    runSingleTest("testNoOperationsOnALineInTheMiddle");
    runSingleTest("testLineEndings");
    runSingleTest("testDifferentLineEndings");
    runSingleTest("testFullLineDelete");
    runSingleTest("testInterlineDelete");
    runSingleTest("testdelFirstLine");
    //runSingleTest("testChangeFullLine");
    runSingleTest("testChangeFullLastLine");
    runSingleTest("testInsertLine");

    runSingleTest("testDifference");
}

function runSingleTest(testName) {
    try {
        eval(testName + "()");
        console.info(testName + " is OK");
    }
    catch (e) {
        console.error(testName + ": " + JSON.stringify(e));
    }
}

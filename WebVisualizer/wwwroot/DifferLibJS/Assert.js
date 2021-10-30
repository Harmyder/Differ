class Assert {
    static AreEqual(expected, actual) {
        if (expected != null || actual != null) {
            if (JSON.stringify(expected) !== JSON.stringify(actual)) {
                throw "";
            }
        }
    }

    static IsTrue(value) {
        if (!value) {
            throw "";
        }
    }
}

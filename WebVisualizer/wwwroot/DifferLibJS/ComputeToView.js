function computeToView(table, before, after) {
    const [deletes, inserts] = Differ.compute(before, after);

    const highlighted = Highlighter.highlight(before, after, deletes, inserts);

    for (let i = 0, beforeIndex = 1, afterIndex = 1; i < highlighted.length; i++) {
        const tr = table.insertRow();
        beforeIndex += AddHalf(tr, beforeIndex, highlighted[i].before, "deleteLine", "deleteBlock");
        afterIndex += AddHalf(tr, afterIndex, highlighted[i].after, "insertLine", "insertBlock");
    }
}

function AddHalf(tr, index, container, lineClass, blockClass) {
    if (container == null) {
        tr.insertCell();
        tr.insertCell();
        return 0;
    }
    else {
        const tdNumber = tr.insertCell();
        tdNumber.className = "lineNumberBlock";
        tdNumber.appendChild(document.createTextNode(index));
        const tdContent = tr.insertCell();
        tdContent.className = container.blocks.length > 1 ? lineClass : "";
        for (let b = 0; b < container.blocks.length; ++b) {
            const span = document.createElement('span');
            if (b % 2 === 1) {
                span.className = blockClass;
            }
            span.innerHTML = container.blocks[b];
            tdContent.appendChild(span);
        }
        return 1;
    }
}

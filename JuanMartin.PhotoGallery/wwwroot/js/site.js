// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function SetRank() {
    var getSelectedValue = document.querySelector('input[name="ranking"]:checked');

    if (getSelectedValue != null) {
        var rank = 11 - parseInt(getSelectedValue.value);
        var rankText = document.getElementById('rank');

        rankText.value = rank;
        document.getElementById('selectedTagListAction').value = "none";
    }
}

function ClearTag() {
    document.getElementById('Tag').value = "";
}

function SetTagListAction(actionName) {
    document.getElementById('selectedTagListAction').value = actionName;
}

function SetCartItemAction(actionName) {
    document.getElementById('currentImageCartAction').value = actionName;
}

function SetTag(element) {
    var selectedText = element.value;

    //alert(selectedText);
    document.getElementById('Tag').value = selectedText;
}

function GetSelectedTagText(element) {
    var selectedText = element.options[element.selectedIndex].text;
    document.getElementById('Tag').value = selectedText;
    document.getElementById('hiddenTag').value = selectedText;
}

//dragndrop
const items = document.querySelectorAll('.item')
items.forEach(item => {
    item.addEventListener('dragstart', dragStart)
    item.addEventListener('dragend', dragEnd)
});
const column = document.querySelector('.column')
new Sortable(column, {
    animation: 150,
    ghostClass: 'blue-background-class'
});

column.addEventListener('dragover', dragOver);
column.addEventListener('dragenter', dragEnter);
column.addEventListener('dragleave', dragLeave);
column.addEventListener('drop', dragDrop);


function dragOver(e) {
    e.preventDefault()
    console.log('drag over');
}
function dragEnter() {
    console.log('drag entered');
}
function dragLeave() {
    console.log('drag left');
}
let dragItem = null;

function dragStart(e) {
    console.log('drag started');
    dragItem = this;
    //	e.dataTransfer.effectAllowed = 'move';
    setTimeout(() => this.className = 'invisible', 0)
    var index = this.getAttribute("name") + ":" + this.getAttribute("index");
    document.getElementById('dragImageIndex').value = index;
}

function dragEnd() {
    console.log('drag ended');
    this.className = 'item'
    dragItem = null;
}

function dragDrop() {
    console.log('drag dropped...');

    var index = this.getAttribute("name") + ":" + this.getAttribute("index");
    document.getElementById('dropImageIndex').value = index;
}
function SubmitOrderIndices() {
    document.getElementById('currentImageCartAction').value = "update";

    var indices = "";
    var list = document.getElementById("orderList");
    var items = list.getElementsByClassName("orderItem");

    for (var i = 0; i < items.length; ++i) {
        // do something with items[i], which is a <div id="orderItem"> element
        if (i > 0)
            indices += ",";
        indices += items[i].getAttribute("name") + ":" + (i + 1).toString();
    }
    document.getElementById('cartItemsSequence').value = indices;

    this.form.submit();
}
function ConfirmCancel() {
    var remove = confirm("Are you sure you want to delete the current order?");
    console.log(remove);

    return remove;
}

// ToolGood.Words.StringSearch.js
// 2020, Lin Zhijun, https://github.com/toolgood/ToolGood.Words
// Licensed under the Apache License 2.0
function StringSearch() {
    function TrieNode() {
        this.Index = 0;
        this.Layer = 0;
        this.End = false;
        this.Char = '';
        this.Results = [];
        this.m_values = {};
        this.Failure = null;
        this.Parent = null;

        this.Add = function (c) {
            if (this.m_values[c] != null) {
                return this.m_values[c];
            }
            var node = new TrieNode();
            node.Parent = this;
            node.Char = c;
            this.m_values[c] = node;
            return node;
        }

        this.SetResults = function (index) {
            if (this.End == false) {
                this.End = true;
            }
            this.Results.push(index)
        }
    }
    function TrieNode2() {
        this.End = false;
        this.Results = [];
        this.m_values = {};
        this.minflag = 0xffff;
        this.maxflag = 0;

        this.Add = function (c, node3) {
            if (this.minflag > c) { this.minflag = c; }
            if (this.maxflag < c) { this.maxflag = c; }
            this.m_values[c] = node3;
        }
        this.SetResults = function (index) {
            if (this.End == false) {
                this.End = true;
            }
            if (this.Results.indexOf(index) == -1) {
                this.Results.push(index);
            }
        }
        this.HasKey = function (c) {
            return this.m_values[c] != undefined;
        }
        this.TryGetValue = function (c) {
            if (this.minflag <= c && this.maxflag >= c) {
                return this.m_values[c];
            }
            return null;
        }
    }
    function swap(A, i, j) { const t = A[i]; A[i] = A[j]; A[j] = t; }
    function divide(A, p, r) {
        const x = A[r - 1].Layer;
        let i = p - 1;
        for (let j = p; j < r - 1; j++) {
            if (A[j].Layer <= x) {
                i++;
                swap(A, i, j);
            }
        }
        swap(A, i + 1, r - 1);
        return i + 1;
    }
    function qsort(A, p = 0, r) {
        r = r || A.length;
        if (p < r - 1) {
            const q = divide(A, p, r);
            qsort(A, p, q);
            qsort(A, q + 1, r);
        }
        return A;
    }
    function quickSort(arr) {
        if (arr.length <= 1) { return arr; }
        return qsort(arguments, 0, arguments.length);
    }

    var _first = [];
    var _keywords = [];

    /**
     * 设置关键字
     * @param {any} keywords
     */
    this.SetKeywords = function (keywords) {
        _keywords = keywords;
        SetKeywords2();
    }
    function SetKeywords2() {
        var root = new TrieNode();
        var allNode = [];
        allNode.push(root);

        for (var i = 0; i < _keywords.length; i++) {
            var p = _keywords[i];
            var nd = root;
            for (var j = 0; j < p.length; j++) {
                nd = nd.Add(p.charCodeAt(j));
                if (nd.Layer == 0) {
                    nd.Layer = j + 1;
                    allNode.push(nd);
                }
            }
            nd.SetResults(i);
        }

        var nodes = [];
        for (var key in root.m_values) {
            if (root.m_values.hasOwnProperty(key) == false) { continue; }
            var nd = root.m_values[key];
            nd.Failure = root;
            for (const trans in nd.m_values) {
                if (nd.m_values.hasOwnProperty(trans) == false) { continue; }
                nodes.push(nd.m_values[trans]);
            }
        }

        // other nodes - using BFS
        while (nodes.length != 0) {
            var newNodes = [];
            for (var key in nodes) {
                if (nodes.hasOwnProperty(key) == false) { continue; }
                var nd = nodes[key];
                var r = nd.Parent.Failure;
                var c = nd.Char;
                while (r != null && !r.m_values[c])
                    r = r.Failure;
                if (r == null)
                    nd.Failure = root;
                else {
                    nd.Failure = r.m_values[c];
                    for (const key2 in nd.Failure.Results) {
                        if (nd.Failure.Results.hasOwnProperty(key2) == false) { continue; }
                        var result = nd.Failure.Results[key2];
                        nd.SetResults(result);
                    }
                }
                for (const key2 in nd.m_values) {
                    if (nd.m_values.hasOwnProperty(key2) == false) { continue }
                    var child = nd.m_values[key2];
                    newNodes.push(child);
                }
            }
            nodes = newNodes;
        }
        root.Failure = root;

        allNode = quickSort(allNode)[0];
        for (var i = 0; i < allNode.length; i++) { allNode[i].Index = i; }

        var allNode2 = [];
        for (var i = 0; i < allNode.length; i++) {
            allNode2.push(new TrieNode2());
        }
        for (var i = 0; i < allNode2.length; i++) {
            var oldNode = allNode[i];
            var newNode = allNode2[i];

            for (const key in oldNode.m_values) {
                if (oldNode.m_values.hasOwnProperty(key) == false) { continue; }
                var index = oldNode.m_values[key].Index;
                newNode.Add(key, allNode2[index]);
            }
            for (let index = 0; index < oldNode.Results.length; index++) {
                const item = oldNode.Results[index];
                newNode.SetResults(item);
            }

            if (oldNode.Failure != root) {
                for (const key in oldNode.Failure.m_values) {
                    if (oldNode.Failure.m_values.hasOwnProperty(key) == false) { continue; }
                    if (newNode.HasKey(key) == false) {
                        var index = oldNode.Failure.m_values[key].Index;
                        newNode.Add(key, allNode2[index]);
                    }
                }
                for (let index = 0; index < oldNode.Failure.Results.length; index++) {
                    const item = oldNode.Failure.Results[index];
                    newNode.SetResults(item);
                }
            }
        }
        allNode = null;
        root = null;

        var first = [];
        for (let index = 0; index < 0xffff; index++) {
            first.push(null);
        }
        for (const key in allNode2[0].m_values) {
            if (allNode2[0].m_values.hasOwnProperty(key) == false) { continue; }
            first[key] = allNode2[0].m_values[key];
        }
        _first = first;
    }
    /**
     * 查找第一个匹配 字符串
     * @param {any} text
     */
    this.FindFirst = function (text) {
        var ptr = null;
        for (let index = 0; index < text.length; index++) {
            const t = text.charCodeAt(index);
            var tn = null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                tn = ptr.TryGetValue(t);
                if (!tn) {
                    tn = _first[t];
                }
            }
            if (tn != null) {
                if (tn.End) {
                    return _keywords[tn.Results[0]];
                }
            }
            ptr = tn;
        }
        return null;
    }

    /**
     * 查找所有匹配 字符串
     * @param {any} text
     */
    this.FindAll = function (text) {
        var ptr = null;
        var list = [];

        for (let index = 0; index < text.length; index++) {
            const t = text.charCodeAt(index);
            var tn = null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                tn = ptr.TryGetValue(t);
                if (!tn) {
                    tn = _first[t];
                }
            }
            if (tn != null) {
                if (tn.End) {
                    for (let j = 0; j < tn.Results.length; j++) {
                        const item = tn.Results[j];
                        list.push(_keywords[item]);
                    }
                }
            }
            ptr = tn;
        }
        return list;
    }

    /**
     * 检查是否包含
     * @param {any} text
     */
    this.ContainsAny = function (text) {
        var ptr = null;
        for (let index = 0; index < text.length; index++) {
            const t = text.charCodeAt(index);
            var tn = null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                tn = ptr.TryGetValue(t);
                if (!tn) {
                    tn = _first[t];
                }
            }
            if (tn != null) {
                if (tn.End) {
                    return true;
                }
            }
            ptr = tn;
        }
        return false;
    }

    /**
     * 查找所有匹配全部替换
     * @param {any} text
     * @param {any} replaceChar
     */
    this.Replace = function (text, replaceChar = '*') {
        var result = text.split('');

        var ptr = null;
        for (var i = 0; i < text.length; i++) {
            const t = text.charCodeAt(i);

            var tn = null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                tn = ptr.TryGetValue(t);
                if (!tn) {
                    tn = _first[t];
                }
            }
            if (tn != null) {
                if (tn.End) {
                    var maxLength = _keywords[tn.Results[0]].length;
                    var start = i + 1 - maxLength;
                    for (var j = start; j <= i; j++) {
                        result[j] = replaceChar;
                    }
                }
            }
            ptr = tn;
        }
        return result.join("");
    }
}
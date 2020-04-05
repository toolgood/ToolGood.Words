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
            return m_values[c] != undefined;
        }
        this.TryGetValue = function (c) {
            if (minflag <= c && maxflag >= c) {
                return m_values[c];
            }
            return null;
        }
    }
    function quickSort(arr) {
        if (arr.length <= 1) {
            return arr;
        }
        var pivotIndex = Math.floor(arr.length / 2);
        var pivot = arr.splice(pivotIndex, 1)[0];
        var left = [];
        var right = [];

        for (var i = 0; i < arr.length; i++) {
            if (arr[i].Layer < pivot.Layer) {
                left.push(arr[i]);
            } else {
                right.push(arr[i]);
            }
        }
        return quickSort(left).concat([pivot], quickSort(right));
    };
    var _first = [];
    var _keywords = [];

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
                nd = nd.push(p.charCodeAt(j));
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
                if (root.m_values.hasOwnProperty(trans) == false) { continue; }
                nodes.push(nd.m_values[trans]);
            }
        }

        // other nodes - using BFS
        while (nodes.length != 0) {
            var newNodes = [];
            for (var key in nodes) {
                if (nodes.hasOwnProperty(key) == false) { continue; }
                var nd = object[key];
                var r = nd.Parent.Failure;
                var c = nd.Char;
                while (r != null && !r.m_values[c]) r = r.Failure;
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
                for (const key in nd.m_values.Values) {
                    if (nd.m_values.Values.hasOwnProperty(key) == false) { continue }
                    newNodes.push(child);
                }
            }
            nodes = newNodes;
        }
        root.Failure = root;

        allNode = quickSort(allNode);
        for (var i = 0; i < allNode.length; i++) { allNode[i].Index = i; }

        var allNode2 = [];
        for (var i = 0; i < allNode.Count; i++) {
            allNode2.Add(new TrieNode2());
        }
        for (var i = 0; i < allNode2.Count; i++) {
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
            first[item.Key] = item.Value;
        }
        _first = first;
    }
    this.FindFirst = function (text) {
        var ptr = null;
        for (let index = 0; index < text.length; index++) {
            const t = array.charCodeAt(index);
            var tn = null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                var nd = ptr.TryGetValue(t);
                if (nd == null) {
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

    this.FindAll = function (text) {
        var ptr = null;
        var list = [];

        for (let index = 0; index < text.length; index++) {
            const t = array.charCodeAt(index);
            var tn = null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                var nd = ptr.TryGetValue(t);
                if (nd == null) {
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

    this.ContainsAny = function (text) {
        var ptr = null;
        for (let index = 0; index < text.length; index++) {
            const t = array.charCodeAt(index);
            var tn = null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                var nd = ptr.TryGetValue(t);
                if (nd == null) {
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

    this.Replace = function (text, replaceChar = '*') {
        var result = text.split('');

        var ptr = null;
        for (var i = 0; i < text.Length; i++) {
            var tn = null;
            if (ptr == null) {
                tn = _first[text[i]];
            } else {
                var nd = ptr.TryGetValue(t);
                if (nd == null) {
                    tn = _first[t];
                }
            }
            if (tn != null) {
                if (tn.End) {
                    var maxLength = _keywords[tn.Results[0]].Length;
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
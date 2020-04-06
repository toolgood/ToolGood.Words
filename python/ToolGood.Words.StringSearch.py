####!/usr/bin/env python
# -*- coding:utf-8 -*-
# ToolGood.Words.Translate.js
# 2020, Lin Zhijun, https://github.com/toolgood/ToolGood.Words
# Licensed under the Apache License 2.0

__all__ = ['StringSearch']
__author__ = 'Lin Zhijun'
__date__ = '2020.04.06'

class TrieNode():
    def __init__():
        self.Index = 0
        self.Index = 0
        self.Layer = 0
        self.End = false
        self.Char = ''
        self.Results = []
        self.m_values = {}
        self.Failure = null
        self.Parent = null

    def Add(c):
        if (self.m_values[c] != null):
            return self.m_values[c]
        node = TrieNode()
        node.Parent = self
        node.Char = c
        self.m_values[c] = node
        return node

    def SetResults(index):
        if (self.End == false):
            self.End = true
        self.Results.append(index)

class TrieNode2():
    def __init__():
        self.End = false
        self.Results = []
        self.m_values = {}
        self.minflag = 0xffff
        self.maxflag = 0

    def Add(c,node3):
        if (self.minflag > c):
            self.minflag = c
        if (self.maxflag < c):
             self.maxflag = c
        self.m_values[c] = node3

    def HasKey(c):
        return self.m_values[c] != undefined
 
    def TryGetValue(c):
        if (self.minflag <= c and self.maxflag >= c):
            return self.m_values[c]
        return null


class StringSearch():
    def __init__():
        self._first = []
        self._keywords = []

    def __swap(A, i, j):
        t = A[i]
        A[i] = A[j]
        A[j] = t

    def __divide(A, p, r):
        x = A[r - 1].Layer
        i = p - 1
        for j in range(p,r - 1): # for (j = p; j < r - 1; j++) 
            if (A[j].Layer <= x):
                i=i+1
                __swap(A, i, j)
        __swap(A, i + 1, r - 1)
        return i + 1

    def __qsort(A, p , r):
        r = r or A.length
        if (p < r - 1):
            q = __divide(A, p, r)
            __qsort(A, p, q)
            __qsort(A, q + 1, r)
        return A
    def quickSort(arr):
        if (arr.length <= 1):
            return arr
        return __qsort(arguments, 0, arguments.length)
    
    def SetKeywords(keywords):
        _keywords = keywords
        root = TrieNode()
        allNode = []
        allNode.append(root)

        for i in range(len(_keywords)): # for (i = 0; i < _keywords.length; i++) 
            p = _keywords[i]
            nd = root
            for j in range(len(p)): # for (j = 0; j < p.length; j++) 
                nd = nd.Add(p.charCodeAt(j))
                if (nd.Layer == 0):
                    nd.Layer = j + 1
                    allNode.append(nd)
            nd.SetResults(i)

        nodes = []
        for  key in root.m_values.keys() :
            if (root.m_values.hasOwnProperty(key) == false):
                continue
            nd = root.m_values[key]
            nd.Failure = root
            for trans in nd.m_values :
                if (nd.m_values.hasOwnProperty(trans) == false):
                    continue
                nodes.append(nd.m_values[trans])
            
        while (nodes.length != 0):
            newNodes = []
            for key in nodes :
                if (nodes.hasOwnProperty(key) == false):
                    continue
                nd = nodes[key]
                r = nd.Parent.Failure
                c = nd.Char
                while (r != null and r.m_values[c] != null):
                    r = r.Failure
                if (r == null):
                    nd.Failure = root
                else:
                    nd.Failure = r.m_values[c]
                    for key2 in nd.Failure.Results :
                        if (nd.Failure.Results.hasOwnProperty(key2) == false):
                            continue
                        result = nd.Failure.Results[key2]
                        nd.SetResults(result)
                    
                
                for key2 in nd.m_values :
                    if (nd.m_values.hasOwnProperty(key2) == false):
                        continue 
                    child = nd.m_values[key2]
                    newNodes.append(child)
            nodes = newNodes
        root.Failure = root

        allNode = quickSort(allNode)[0]
        for i in range(len(allNode)): # for (i = 0; i < allNode.length; i++) 
             allNode[i].Index = i

        allNode2 = []
        for i in range(len(allNode)): # for (i = 0; i < allNode.length; i++) 
            allNode2.append( TrieNode2())
        
        for i in range(len(allNode2)): # for (i = 0; i < allNode2.length; i++) 
            oldNode = allNode[i]
            newNode = allNode2[i]

            for key in oldNode.m_values :
                if (oldNode.m_values.hasOwnProperty(key) == false):
                    continue
                index = oldNode.m_values[key].Index
                newNode.Add(key, allNode2[index])
            
            for index in range(len(oldNode.Results)): # for (index = 0; index < oldNode.Results.length; index++) 
                item = oldNode.Results[index]
                newNode.SetResults(item)
            

            if (oldNode.Failure != root):
                for key in oldNode.Failure.m_values :
                    if (oldNode.Failure.m_values.hasOwnProperty(key) == false):
                        continue
                    if (newNode.HasKey(key) == false):
                        index = oldNode.Failure.m_values[key].Index
                        newNode.Add(key, allNode2[index])
                    
                for index in range(len(oldNode.Failure.Results)): # for (index = 0; index < oldNode.Failure.Results.length; index++) 
                    item = oldNode.Failure.Results[index]
                    newNode.SetResults(item)
        allNode = null
        root = null

        first = []
        for index in range(65535):# for (index = 0; index < 0xffff; index++) 
            first.append(null)
        
        for key in allNode2[0].m_values :
            if (allNode2[0].m_values.hasOwnProperty(key) == false):
                continue
            first[key] = allNode2[0].m_values[key]
        
        self._first = first
    

    def FindFirst(text):
        ptr = null
        for index in range(len(text)): # for (index = 0; index < text.length; index++) 
            t = text.charCodeAt(index)
            tn = null
            if (ptr == null):
                tn = self._first[t]
            else:
                tn = ptr.TryGetValue(t)
                if (tn==null):
                    tn = self._first[t]
                
            
            if (tn != null):
                if (tn.End):
                    return _keywords[tn.Results[0]]
            ptr = tn
        return null

    def FindAll(text):
        ptr = null
        list = []

        for index in range(len(text)): # for (index = 0; index < text.length; index++) 
            t = text.charCodeAt(index)
            tn = null
            if (ptr == null):
                tn = self._first[t]
            else:
                tn = ptr.TryGetValue(t)
                if (tn==null):
                    tn = self._first[t]
                
            
            if (tn != null):
                if (tn.End):
                    for j in range(len(tn.Results)): # for (j = 0; j < tn.Results.length; j++) 
                        item = tn.Results[j]
                        list.append(_keywords[item])
            ptr = tn
        return list


    def ContainsAny(text):
        ptr = null
        for index in range(len(text)): # for (index = 0; index < text.length; index++) 
            t = text.charCodeAt(index)
            tn = null
            if (ptr == null):
                tn = self._first[t]
            else:
                tn = ptr.TryGetValue(t)
                if (tn==null):
                    tn = self._first[t]
            
            if (tn != null):
                if (tn.End):
                    return true
            ptr = tn
        return false
    
    def Replace(text, replaceChar = '*'):
        result = text.split('')

        ptr = null
        for i in range(len(text)): # for (i = 0; i < text.length; i++) 
            t = text.charCodeAt(i)

            tn = null
            if (ptr == null):
                tn = self._first[t]
            else:
                tn = ptr.TryGetValue(t)
                if (tn==null):
                    tn = self._first[t]
            
            if (tn != null):
                if (tn.End):
                    maxLength = _keywords[tn.Results[0]].length
                    start = i + 1 - maxLength
                    for j in range(start,i+1): # for (j = start; j <= i; j++) 
                        result[j] = replaceChar
            ptr = tn
        return result.join("")
    


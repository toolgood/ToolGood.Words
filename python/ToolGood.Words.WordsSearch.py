#!/usr/bin/env python
# -*- coding:utf-8 -*-
# ToolGood.Words.WordsSearch.py
# 2020, Lin Zhijun, https://github.com/toolgood/ToolGood.Words
# Licensed under the Apache License 2.0
import sys
sys.setrecursionlimit(1000000)

__all__ = ['WordsSearch']
__author__ = 'Lin Zhijun'
__date__ = '2020.04.06'

class TrieNode():
    def __init__(self):
        self.Index = 0
        self.Index = 0
        self.Layer = 0
        self.End = False
        self.Char = ''
        self.Results = []
        self.m_values = {}
        self.Failure = None
        self.Parent = None

    def Add(self,c):
        if c in self.m_values :
            return self.m_values[c]
        node = TrieNode()
        node.Parent = self
        node.Char = c
        self.m_values[c] = node
        return node

    def SetResults(self,index):
        if (self.End == False):
            self.End = True
        self.Results.append(index)

class TrieNode2():
    def __init__(self):
        self.End = False
        self.Results = []
        self.m_values = {}
        self.minflag = 0xffff
        self.maxflag = 0

    def Add(self,c,node3):
        if (self.minflag > c):
            self.minflag = c
        if (self.maxflag < c):
             self.maxflag = c
        self.m_values[c] = node3

    def SetResults(self,index):
        if (self.End == False) :
            self.End = True
        if (index in self.Results )==False : 
            self.Results.append(index)

    def HasKey(self,c):
        return c in self.m_values
        
 
    def TryGetValue(self,c):
        if (self.minflag <= c and self.maxflag >= c):
            if c in self.m_values:
                return self.m_values[c]
        return None


class WordsSearch():
    def __init__(self):
        self._first = []
        self._keywords = []
        self._indexs=[]
    
    def SetKeywords(self,keywords):
        self._keywords = keywords
        self._indexs=[]
        for i in range(len(keywords)):
            self._indexs.append(i)

        root = TrieNode()
        allNode = []
        allNode.append(root)
        allNodeLayer={}

        for i in range(len(self._keywords)): # for (i = 0; i < _keywords.length; i++) 
            p = self._keywords[i]
            nd = root
            for j in range(len(p)): # for (j = 0; j < p.length; j++) 
                nd = nd.Add(ord(p[j]))
                if (nd.Layer == 0):
                    nd.Layer = j + 1
                    if nd.Layer in allNodeLayer:
                        allNodeLayer[nd.Layer].append(nd);
                    else:
                        allNodeLayer[nd.Layer]=[]
                        allNodeLayer[nd.Layer].append(nd);
            nd.SetResults(i)

        for key in allNodeLayer.keys():
            for nd in allNodeLayer[key]:
                allNode.append(nd)

        nodes = []
        for  key in root.m_values.keys() :
            nd = root.m_values[key]
            nd.Failure = root
            for trans in nd.m_values :
                nodes.append(nd.m_values[trans])
            
        while len(nodes) != 0:
            newNodes = []
            for nd in nodes :
                #nd = nodes[key]
                r = nd.Parent.Failure
                c = nd.Char
                while (r != None and (c in r.m_values)==False):
                    r = r.Failure
                if (r == None):
                    nd.Failure = root
                else:
                    nd.Failure = r.m_values[c]
                    for key2 in nd.Failure.Results :
                        nd.SetResults(key2)
                    
                
                for key2 in nd.m_values :
                    child = nd.m_values[key2]
                    newNodes.append(child)
            nodes = newNodes
        root.Failure = root

        for i in range(len(allNode)): # for (i = 0; i < allNode.length; i++) 
             allNode[i].Index = i

        allNode2 = []
        for i in range(len(allNode)): # for (i = 0; i < allNode.length; i++) 
            allNode2.append( TrieNode2())
        
        for i in range(len(allNode2)): # for (i = 0; i < allNode2.length; i++) 
            oldNode = allNode[i]
            newNode = allNode2[i]

            for key in oldNode.m_values :
                index = oldNode.m_values[key].Index
                newNode.Add(key, allNode2[index])
            
            for index in range(len(oldNode.Results)): # for (index = 0; index < oldNode.Results.length; index++) 
                item = oldNode.Results[index]
                newNode.SetResults(item)
            

            if (oldNode.Failure != root):
                for key in oldNode.Failure.m_values :
                    if (newNode.HasKey(key) == False):
                        index = oldNode.Failure.m_values[key].Index
                        newNode.Add(key, allNode2[index])
                    
                for index in range(len(oldNode.Failure.Results)): # for (index = 0; index < oldNode.Failure.Results.length; index++) 
                    item = oldNode.Failure.Results[index]
                    newNode.SetResults(item)
        allNode = None
        root = None

        first = []
        for index in range(65535):# for (index = 0; index < 0xffff; index++) 
            first.append(None)
        
        for key in allNode2[0].m_values :
            first[key] = allNode2[0].m_values[key]
        
        self._first = first
    

    def FindFirst(self,text):
        ptr = None
        for index in range(len(text)): # for (index = 0; index < text.length; index++) 
            t =ord(text[index]) # text.charCodeAt(index)
            tn = None
            if (ptr == None):
                tn = self._first[t]
            else:
                tn = ptr.TryGetValue(t)
                if (tn==None):
                    tn = self._first[t]
                
            
            if (tn != None):
                if (tn.End):
                    item = tn.Results[0]
                    keyword = self._keywords[item]
                    return { "Keyword": keyword, "Success": True, "End": index, "Start": index + 1 - len(keyword), "Index": self._indexs[item] }
            ptr = tn
        return None

    def FindAll(self,text):
        ptr = None
        list = []

        for index in range(len(text)): # for (index = 0; index < text.length; index++) 
            t =ord(text[index]) # text.charCodeAt(index)
            tn = None
            if (ptr == None):
                tn = self._first[t]
            else:
                tn = ptr.TryGetValue(t)
                if (tn==None):
                    tn = self._first[t]
                
            
            if (tn != None):
                if (tn.End):
                    for j in range(len(tn.Results)): # for (j = 0; j < tn.Results.length; j++) 
                        item = tn.Results[j]
                        keyword = self._keywords[item]
                        list.append({ "Keyword": keyword, "Success": True, "End": index, "Start": index + 1 - len(keyword), "Index": self._indexs[item] })
            ptr = tn
        return list


    def ContainsAny(self,text):
        ptr = None
        for index in range(len(text)): # for (index = 0; index < text.length; index++) 
            t =ord(text[index]) # text.charCodeAt(index)
            tn = None
            if (ptr == None):
                tn = self._first[t]
            else:
                tn = ptr.TryGetValue(t)
                if (tn==None):
                    tn = self._first[t]
            
            if (tn != None):
                if (tn.End):
                    return True
            ptr = tn
        return False
    
    def Replace(self,text, replaceChar = '*'):
        result = list(text) 

        ptr = None
        for i in range(len(text)): # for (i = 0; i < text.length; i++) 
            t =ord(text[i]) # text.charCodeAt(index)
            tn = None
            if (ptr == None):
                tn = self._first[t]
            else:
                tn = ptr.TryGetValue(t)
                if (tn==None):
                    tn = self._first[t]
            
            if (tn != None):
                if (tn.End):
                    maxLength = len( self._keywords[tn.Results[0]])
                    start = i + 1 - maxLength
                    for j in range(start,i+1): # for (j = start; j <= i; j++) 
                        result[j] = replaceChar
            ptr = tn
        return ''.join(result) 

if __name__ == "__main__":
    s = "中国|国人|zg人"
    test = "我是中国人"


    search = WordsSearch()
    search.SetKeywords(s.split('|'))

    print("-----------------------------------  WordsSearch  -----------------------------------" )

    print("WordsSearch FindFirst is run.")
    f = search.FindFirst(test)
    if f["Keyword"]!="中国" :
        print("WordsSearch FindFirst is error.............................")
 
    print("WordsSearch FindAll is run.")
    all = search.FindAll(test)
    if all[0]["Keyword"]!="中国" :
        print("WordsSearch FindAll is error.............................")
    if all[1]["Keyword"]!="国人" :
        print("WordsSearch FindAll is error.............................")
    if all[0]["Start"]!=2 :
        print("WordsSearch FindAll is error.............................")
    if all[0]["End"]!=3 :
        print("WordsSearch FindAll is error.............................")
    if len(all)!=2 :
        print("WordsSearch FindAll is error.............................")

    print("WordsSearch ContainsAny is run.")
    b = search.ContainsAny(test)
    if b==False :
        print("WordsSearch ContainsAny is error.............................")

    print("WordsSearch Replace  is run.")
    txt = search.Replace (test)
    if (txt != "我是***"):
        print("WordsSearch Replace  is error.............................")

    print("-----------------------------------  Test End  -----------------------------------") 



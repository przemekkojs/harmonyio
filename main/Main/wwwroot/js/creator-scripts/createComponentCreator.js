import { Component } from "./component.js";

export function createComponent(newId) {
    const component = new Component(newId);

    return component;
}
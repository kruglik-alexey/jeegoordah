import React from 'react'

export default function (props) {
    return (
        <div className="header">
            <h1>{props.children}</h1>
        </div>
    );
}
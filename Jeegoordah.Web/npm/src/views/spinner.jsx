import React from 'react'

export default function(props) {
    const size = props.size || 24;
    return (
        <div className="spinner">
            <i className="fa fa-spinner fa-spin" style={{fontSize: `${size}px`}}></i>
        </div>
    )
}
import React from 'react'

export default function () {
    return (
        <nav className="navbar navbar-default navbar-inverse navbar-fixed-top">
            <div className="navbar-header">
                <button type="button" className="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1" aria-expanded="false">
                    <span className="sr-only">Toggle navigation</span>
                    <span className="icon-bar"></span>
                    <span className="icon-bar"></span>
                    <span className="icon-bar"></span>
                </button>
                <span className="navbar-brand">Jeegoordah</span>
            </div>
            <div className="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
                <ul className="nav navbar-nav">
                    <li className="active"><a>Total</a></li>
                    <li><a>Events</a></li>
                    <li><a>P2P transactions</a></li>
                    <li><a>Return Money</a></li>
                    <li><a>Android</a></li>
                    <li><a>Calendar</a></li>
                </ul>
            </div>
        </nav>
    );
}

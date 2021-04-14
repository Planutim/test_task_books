import React from 'react'
import './DataTable.css'

export default class DataTable extends React.Component {
    constructor(props) {
        super(props)
        this.state = {
            isLoading: false,
            sortBy: "id",
            orderByAsc: true,
            books: []
        }

        this.handleSortAndOrder = this.handleSortAndOrder.bind(this)
        this.fetchData = this.fetchData.bind(this)
    }

    async componentDidMount() {
        
        document.getElementById('id-radio').checked = true
        await this.fetchData()
        if (this.state.books.length === 0) {
            await populateData()
        }
        await this.fetchData()
        
    }

    async fetchData() {
        this.setState({
            isLoading: true
        })
        const uri = "/api/books";
        const sortBy = this.state.sortBy;
        const orderBy = this.state.orderByAsc ? "asc" : "desc";

        var rawResponse = await fetch(`${uri}?sortBy=${sortBy}&orderBy=${orderBy}`)
        if (rawResponse.ok) {
            var books = await rawResponse.json()

            this.setState({
                books,
                isLoading: false,
            })
        }
        else {
            this.setState({
                isLoading: false
            })
        }
    }
    handleSortAndOrder(e) {
        //alert(e.target.value)
        //return
        if (this.state.isLoading) {
            return
        }
        if (this.state.sortBy === e.target.value) {
            this.setState({
                orderByAsc: !this.state.orderByAsc
            }, this.fetchData)
        } else {
            this.setState({
                sortBy: e.target.value,
                orderByAsc: true
            }, this.fetchData)
        }
    }

    async buyABook(id) {
        var response = await fetch(`/api/books/${id}`, {
            method: "DELETE"
        })
        if (response.ok) {
            this.setState({
                books : this.state.books.filter(book=>book.id!==id)
            })
        }
    }
    render() {
        const rows = this.state.books.map((book,i) => {
            const formattedDate = new Date(Date.parse(book.published)).toLocaleDateString()
            return (
                <tr key={i}>
                    <td>{book.id}</td>
                    <td>{book.name}</td>
                    <td>{book.author}</td>
                    <td>{formattedDate}</td>
                    <td>
                        <button onClick={() => { this.buyABook(book.id) } }>Купить</button>
                    </td>
                </tr>
            )
        })
        return (
            <table className="table table-bordered text-center ">
                <thead className="thead-light">
                    <tr>
                        <th>
                            <input
                                className={this.state.orderByAsc ? "input-asc" : "input-desc"}
                                type="radio"
                                name="sortBy"
                                id="id-radio"
                                value="id"
                                onClick={this.handleSortAndOrder}
                            />
                            <label htmlFor="id-radio">Id</label>
                        </th>
                        <th>
                            <input
                                className={this.state.orderByAsc ? "input-asc" : "input-desc"}
                                type="radio"
                                name="sortBy"
                                id="name-radio"
                                value="name"
                                onClick={this.handleSortAndOrder}
                            />
                            <label htmlFor="name-radio">Name</label>
                        </th>
                        <th>
                            <input
                                className={this.state.orderByAsc ? "input-asc" : "input-desc"}
                                type="radio"
                                name="sortBy"
                                id="author-radio"
                                value="author"
                                onClick={this.handleSortAndOrder}
                            />
                            <label htmlFor="author-radio">Author</label>
                        </th>
                        <th>                        
                            <input
                                className={this.state.orderByAsc ? "input-asc" : "input-desc"}
                                type="radio"
                                name="sortBy"
                                id="published-radio"
                                value="published"
                                onClick={this.handleSortAndOrder}
                            />
                            <label htmlFor="published-radio">Published</label>
                        </th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>)
    }
}


async function populateData() {
    let uri = "api/books/populate"
    await fetch(uri, {
        method: "post"
    })
}

import { useState } from "react";

export default function PaymentDetails() {

    const [cardNumber, setCardNumber] = useState("");
    const [expDate, setExpDate] = useState("");
    const [name, setName] = useState("");

    const handleExpDate = (e: React.ChangeEvent<HTMLInputElement>) => {
        setExpDate(e.target.value)
    }

    const handleNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setName(e.target.value);
    }

    const sendCardDetails = async () => {
        const cartId = localStorage.getItem("cart_key");

        const data = {
            cartId: cartId ?? "",
            cardNumber: cardNumber,
            expirationDate: new Date(expDate).toISOString(),
            cardHolderName: name,
            cvc: 111
        }

        const res = await fetch(`${process.env.REACT_APP_API_URL}/payment`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data)
        });
        
        setCardNumber("");
        setExpDate("");
        setName("");
    }

    return (
        <div>
            <div>
                <label>Card Number</label>
                <input type="text" onChange={e => setCardNumber(e.target.value.replace(/\D/, ''))} value={cardNumber} />
            </div>
            <div>
                <label>Expiration Date</label>
                <input type="text" onChange={handleExpDate} value={expDate} />
            </div>
            <div>
                <label>Name on Card</label>
                <input type="text" onChange={handleNameChange} value={name}/>
            </div>
            <button onClick={() => sendCardDetails()}>Submit Payment</button>
        </div>
    )
}
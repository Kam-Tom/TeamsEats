import React, { useState, useContext, ChangeEvent } from 'react';
import {
    Dialog,
    DialogTrigger,
    DialogSurface,
    DialogTitle,
    DialogBody,
    DialogActions,
    DialogContent,
    Button,
    Field,
    Input,
    makeStyles
} from "@fluentui/react-components";
import { TeamsFxContext } from '../Context';

interface FormData {
    dishName: string;
    price: string;
    additionalInfo: string;
    groupOrderId: number;
    itemId: number;
}

interface FormErrors {
    dishName?: string;
    price?: string;
}

interface OrderItemFormProps {
    dishName: string;
    price: string;
    additionalInfo: string;
    groupOrderId: number;
    itemId: number;
}

const useStyles = makeStyles({
    formSurface: {
        width: '400px',
    },
    formContent: {
        display: 'flex',
        flexDirection: 'column',
        gap: '10px',
    },
    field: {
        marginBottom: '10px',
    },
    formButtons: {
        display: 'flex',
        justifyContent: 'flex-end',
        gap: '10px',
    },
});

const EditForm: React.FC<OrderItemFormProps> = ({ groupOrderId, itemId, dishName, price, additionalInfo }) => {
    const { teamsUserCredential } = useContext(TeamsFxContext);
    const [formData, setFormData] = useState<FormData>({
        dishName: dishName,
        price: price,
        additionalInfo: additionalInfo,
        itemId: itemId,
        groupOrderId: groupOrderId // Use the passed groupOrderId
    });
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [errors, setErrors] = useState<FormErrors>({});
    const classes = useStyles();

    const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = event.target;
        setFormData({ ...formData, [name]: value });
    };

    const validateForm = (): boolean => {
        const newErrors: FormErrors = {};
        if (!formData.dishName) {
            newErrors.dishName = 'Dish name is required.';
        }
        if (!formData.price || parseFloat(formData.price) <= 0) {
            newErrors.price = 'Price must be a positive number.';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async () => {
        if (!validateForm()) return;

        try {
            if (!teamsUserCredential) return;
            const token = await teamsUserCredential.getToken("");

            const formattedData = {
                dishName: formData.dishName,
                price: parseFloat(formData.price),
                additionalInfo: formData.additionalInfo,
                groupOrderId: groupOrderId,
                orderItemID: itemId
            };

            const response = await fetch('https://localhost:7125/OrderItem', {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token!.token}`
                },
                body: JSON.stringify(formattedData)
            });

            if (response.ok) {
                setIsDialogOpen(false);
            } else {
                console.error(`Failed to submit form: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during form submission:', error);
        }
    };

    return (
        <Dialog open={isDialogOpen} onOpenChange={(_, data) => setIsDialogOpen(data.open)}>
            <DialogTrigger disableButtonEnhancement>
                <Button onClick={() => setIsDialogOpen(true)}>Edit</Button>
            </DialogTrigger>
            <DialogSurface className={classes.formSurface}>
                <DialogBody>
                    <DialogTitle>Add Order Item {groupOrderId}</DialogTitle>
                    <DialogContent className={classes.formContent}>
                        <Field className={classes.field}
                            label="Dish Name"
                            validationState={errors.dishName ? 'error' : 'none'}
                            validationMessage={errors.dishName}
                        >
                            <Input
                                id="dishName"
                                name="dishName"
                                value={formData.dishName}
                                onChange={handleChange}
                                required
                            />
                        </Field>

                        <Field className={classes.field}
                            label="Price"
                            validationState={errors.price ? 'error' : 'none'}
                            validationMessage={errors.price}
                        >
                            <Input
                                id="price"
                                name="price"
                                type="number"
                                value={formData.price}
                                onChange={handleChange}
                                required
                            />
                        </Field>

                        <Field className={classes.field}
                            label="Additional Info"
                        >
                            <Input
                                id="additionalInfo"
                                name="additionalInfo"
                                value={formData.additionalInfo}
                                onChange={handleChange}
                            />
                        </Field>
                    </DialogContent>
                    <DialogActions className={classes.formButtons}>
                        <DialogTrigger disableButtonEnhancement>
                            <Button appearance="secondary">Close</Button>
                        </DialogTrigger>
                        <Button appearance="primary" onClick={handleSubmit}>Submit</Button>
                    </DialogActions>
                </DialogBody>
            </DialogSurface>
        </Dialog>
    );
}

export default EditForm;

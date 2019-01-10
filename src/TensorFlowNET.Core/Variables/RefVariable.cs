﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Tensorflow
{
    public class RefVariable : VariableV1
    {
        public bool _in_graph_mode = true;
        public Tensor _initial_value;
        public string _graph_key;
        public bool _trainable;
        public Tensor _variable;

        public RefVariable(object initial_value,
            bool trainable = true,
            List<string> collections = null,
            bool validate_shape = true,
            string caching_device = "",
            string name = "",
            TF_DataType dtype = TF_DataType.DtInvalid) : 
            base(initial_value, trainable, collections, validate_shape, caching_device, name, dtype)
        {
            _init_from_args(initial_value, trainable, collections, validate_shape, caching_device, name, dtype);
        }

        private void _init_from_args(object initial_value,
            bool trainable = true,
            List<string> collections = null,
            bool validate_shape = true,
            string caching_device = "",
            string name = "",
            TF_DataType dtype = TF_DataType.DtInvalid)
        {
            if (initial_value is null)
                throw new ValueError("initial_value must be specified.");

            var init_from_fn = false;

            if(collections == null)
            {
                collections = new List<string> { ops.GraphKeys.GLOBAL_VARIABLES };
            }

            // Store the graph key so optimizers know how to only retrieve variables from
            // this graph.
            _graph_key = ops.get_default_graph()._graph_key;

            _trainable = trainable;
            if (!collections.Contains(ops.GraphKeys.TRAINABLE_VARIABLES))
                collections.Add(ops.GraphKeys.TRAINABLE_VARIABLES);

            ops.init_scope();
            name = new ops.name_scope(name, "Variable", init_from_fn ? new List<object>() : new List<object> { initial_value });
            if (init_from_fn)
            {

            }
            else
            {
                _initial_value = ops.convert_to_tensor(initial_value, name: "initial_value");
            }

            var shape = _initial_value.shape;
            dtype = _initial_value.dtype;
            _variable = gen_state_ops.variable_v2(shape, dtype, name);
        }
    }
}
